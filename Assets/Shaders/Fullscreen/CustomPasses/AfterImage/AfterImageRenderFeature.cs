using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class AfterImageRenderFeature : ScriptableRendererFeature
{
    [SerializeField]
    private AfterImageSettings settings;
    AfterImagePass afterImagePass;

    [SerializeField]
    private RenderPassEvent renderEvent = RenderPassEvent.AfterRenderingPostProcessing;

    // called on render feature activation and deactivation
    public override void Create()
    {
        if (settings.material == null) return;

        settings.material.SetInt("_UseFBF", settings.useFBF ? 1 : 0);
        settings.material.SetInt("_UseGrayscale", settings.useGrayscale ? 1 : 0);

        // TODO:Test with VR
        // if (settings.useFBF)
        //     settings.material.EnableKeyword("_USEFBF_ON");
        // else
        //     settings.material.DisableKeyword("_USEFBF_ON");

        // if (settings.useGrayscale)
        //     settings.material.EnableKeyword("_USEGRAYSCALE_ON");
        // else
        //     settings.material.DisableKeyword("_USEGRAYSCALE_ON");

        afterImagePass = new AfterImagePass(settings);

        // Configures where the render pass should be injected.
        afterImagePass.renderPassEvent = renderEvent;

        // You can request URP color texture and depth buffer as inputs by uncommenting the line below,
        // URP will ensure copies of these resources are available for sampling before executing the render pass.
        // Only uncomment it if necessary, it will have a performance impact, especially on mobiles and other TBDR GPUs where it will break render passes.
        afterImagePass.ConfigureInput(ScriptableRenderPassInput.Color);

        // You can request URP to render to an intermediate texture by uncommenting the line below.
        // Use this option for passes that do not support rendering directly to the backbuffer.
        // Only uncomment it if necessary, it will have a performance impact, especially on mobiles and other TBDR GPUs where it will break render passes.
        afterImagePass.requiresIntermediateTexture = true;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.material == null || renderingData.cameraData.cameraType != CameraType.Game) return;
        renderer.EnqueuePass(afterImagePass);
    }

    // called when the game stops
    protected override void Dispose(bool disposing)
    {
        afterImagePass?.Dispose();
    }

    // Use this class to pass around settings from the feature to the pass
    [Serializable]
    public class AfterImageSettings
    {
        public Material material;
        public bool useFBF;
        public bool useGrayscale;
    }

    class AfterImagePass : ScriptableRenderPass
    {
        private const string _passName = "AfterImagePass";

        readonly AfterImageSettings settings;
        private RTHandle m_HistoryRT;
        private bool isFirstFrame = true;

        public AfterImagePass(AfterImageSettings settings)
        {
            this.settings = settings;
        }

        private class PassData
        {
            internal Material material;
            internal bool usingFBF;
            internal TextureHandle activeColorTexture;
        }


        // This static method is passed as the RenderFunc delegate to the RenderGraph render pass.
        // It is used to execute draw commands.
        // What the GPU execute
        static void ExecutePass(PassData data, RasterGraphContext context)
        {
            if (data.usingFBF)
            {
                // dont need to specify the source in FBF because is well known
                Blitter.BlitTexture(context.cmd, new Vector4(1, 1, 0, 0), data.material, 0);
            }
            else
            {
                Blitter.BlitTexture(context.cmd, data.activeColorTexture, new Vector4(1, 1, 0, 0), data.material, 0);
            }
        }

        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameContext)
        {

            VolumeStack stack = VolumeManager.instance.stack;
            AfterImageEffect effect = stack.GetComponent<AfterImageEffect>();

            if (!effect.IsActive())
            {
                return;
            }

            float blendAmount = effect.GetIntensity();

            // UniversalResourceData contains all the texture handles used by the renderer, including the active color and depth textures
            // The active color and depth textures are the main color and depth buffers that the camera renders into
            UniversalResourceData resourceData = frameContext.Get<UniversalResourceData>();

            // This should never happen since we set m_Pass.requiresIntermediateTexture = true;
            // Unless you set the render event to AfterRendering, where we only have the BackBuffer. 
            if (resourceData.isActiveTargetBackBuffer)
            {
                Debug.LogError($"Skipping render pass. AfterImageRenderFeature requires an intermediate ColorTexture, we can't use the BackBuffer as a texture input.");
                return;
            }

            TextureHandle source = resourceData.activeColorTexture;

            //grab info(description) of the screen
            TextureDesc desc = renderGraph.GetTextureDesc(source);

            // Allocate persistent history RTHandle if needed or if resolution changed
            if (m_HistoryRT == null || m_HistoryRT.rt.width != desc.width || m_HistoryRT.rt.height != desc.height)
            {
                m_HistoryRT?.Release();
                m_HistoryRT = RTHandles.Alloc(
                    desc.width, desc.height,
                    colorFormat: desc.colorFormat,
                    name: "AfterImage_History"
                );
            }

            // this is needed to that copy to the RTHandle
            TextureHandle historyHandle = renderGraph.ImportTexture(m_HistoryRT);

            if (isFirstFrame)
            {
                // Seed history with current frame so frame 1 isn't black
                renderGraph.AddCopyPass(source, historyHandle, passName: "AfterImage_SeedHistory");
                resourceData.cameraColor = source;
                isFirstFrame = false;
                return; // skip blend on first frame
            }

            TextureDesc destinationDesc = source.GetDescriptor(renderGraph);
            destinationDesc.name = $"AfterImage_{_passName}";
            destinationDesc.clearBuffer = false;

            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

            settings.material.SetFloat("_BlendAmount", blendAmount);
            // I must use the RTHandle because the TextureHandle can be access on RenderFunc where it will be populated
            settings.material.SetTexture("_HistoryTex", m_HistoryRT);

            if (settings.useFBF)
            {
                FBFPass(renderGraph, frameContext, source, destination, historyHandle, blendAmount);
            }
            else
            {
                NormalPass(renderGraph, frameContext, source, destination, historyHandle, blendAmount);
            }

            // FrameData allows to get and set internal pipeline buffers. Here we update the CameraColorBuffer to the texture that we just wrote to in this pass. 
            // Because RenderGraph manages the pipeline resources and dependencies, following up passes will correctly use the right color buffer.
            // This optimization has some caveats. You have to be careful when the color buffer is persistent across frames and between different cameras, such as in camera stacking.
            // In those cases you need to make sure your texture is an RTHandle and that you properly manage the lifecycle of it.
            resourceData.cameraColor = destination;
            // renderGraph.AddCopyPass(destination, source, passName: "Copy Back FF Destination (also using FBF)"); // Alternative more expensive?????????


        }

        private void NormalPass(RenderGraph renderGraph, ContextContainer frameContext, TextureHandle source, TextureHandle destination, TextureHandle history, float blendAmount)
        {
            // render pass (GPU job) 
            // Even if is a simple blit operation i use the RasterRenderPass in order to merge passes since BlitPass is unsafe for some reason
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(_passName, out var passData))
            {
                // populating data
                passData.material = settings.material;
                passData.usingFBF = settings.useFBF;
                passData.activeColorTexture = source;

                // We declare the src as input attachment. This is required for framebuffer fetch. 
                // builder.SetInputAttachment(source, 0, AccessFlags.Read);
                builder.UseTexture(source, AccessFlags.Read);

                // set the final destination of the frame.
                // We cannot use MRT because shader_graph does not support it
                builder.SetRenderAttachment(destination, 0);
                // builder.SetRenderAttachment(history, 1);

                // assign the task
                builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
            }
            // update the RTHandle 
            renderGraph.AddCopyPass(destination, history, passName: "AfterImage_SaveHistory");
        }

        private void FBFPass(RenderGraph renderGraph, ContextContainer frameContext, TextureHandle source, TextureHandle destination, TextureHandle history, float blendAmount)
        {
            // render pass (GPU job) 
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(_passName, out var passData))
            {
                // populating data
                passData.material = settings.material;
                passData.usingFBF = settings.useFBF;
                passData.activeColorTexture = source;

                // We declare the src as input attachment. This is required for framebuffer fetch. 
                builder.SetInputAttachment(source, 0, AccessFlags.Read);
                // builder.UseTexture(source, AccessFlags.Read);

                // set the final destination of the frame.
                // We cannot use MRT because shader_graph does not support it
                builder.SetRenderAttachment(destination, 0);
                // builder.SetRenderAttachment(history, 1);

                // assign the task
                builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
            }
            // update the RTHandle 
            renderGraph.AddCopyPass(destination, history, passName: "AfterImage_SaveHistory");
        }

        public void Dispose()
        {
            m_HistoryRT?.Release();
        }
    }
}