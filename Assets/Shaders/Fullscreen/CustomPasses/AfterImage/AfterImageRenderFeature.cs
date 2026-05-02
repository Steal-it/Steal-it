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
    AfterImagePass m_ScriptablePass;

    [SerializeField]
    private RenderPassEvent renderEvent = RenderPassEvent.AfterRenderingPostProcessing;

    // called on render feature activation and deactivation
    public override void Create()
    {
        if (settings.afterImageMaterial == null) return;

        m_ScriptablePass = new AfterImagePass(settings);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = renderEvent;

        // You can request URP color texture and depth buffer as inputs by uncommenting the line below,
        // URP will ensure copies of these resources are available for sampling before executing the render pass.
        // Only uncomment it if necessary, it will have a performance impact, especially on mobiles and other TBDR GPUs where it will break render passes.
        m_ScriptablePass.ConfigureInput(ScriptableRenderPassInput.Color);

        // You can request URP to render to an intermediate texture by uncommenting the line below.
        // Use this option for passes that do not support rendering directly to the backbuffer.
        // Only uncomment it if necessary, it will have a performance impact, especially on mobiles and other TBDR GPUs where it will break render passes.
        m_ScriptablePass.requiresIntermediateTexture = true;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (settings.afterImageMaterial == null || renderingData.cameraData.cameraType != CameraType.Game) return;
        renderer.EnqueuePass(m_ScriptablePass);
    }

    // called when the game stops
    protected override void Dispose(bool disposing)
    {
        m_ScriptablePass?.Dispose();
    }

    // Use this class to pass around settings from the feature to the pass
    [Serializable]
    public class AfterImageSettings
    {
        public Material afterImageMaterial;
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
            internal float blend;
            internal TextureHandle activeColor;
            internal TextureHandle historyTexture;
        }

        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        // public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        // {
        //     VolumeStack stack = VolumeManager.instance.stack;
        //     AfterImageEffect effect = stack.GetComponent<AfterImageEffect>();

        //     if (!effect.IsActive())
        //     {
        //         return;
        //     }

        //     float blendAmount = effect.GetValue();

        //     // UniversalResourceData contains all the texture handles used by the renderer, including the active color and depth textures
        //     // The active color and depth textures are the main color and depth buffers that the camera renders into
        //     UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

        //     // This should never happen since we set m_Pass.requiresIntermediateTexture = true;
        //     // Unless you set the render event to AfterRendering, where we only have the BackBuffer. 
        //     if (resourceData.isActiveTargetBackBuffer)
        //     {
        //         Debug.LogError($"Skipping render pass. AfterImageRenderFeature requires an intermediate ColorTexture, we can't use the BackBuffer as a texture input.");
        //         return;
        //     }

        //     if (isFirstFrame)
        //     {
        //         last_source = resourceData.activeColorTexture;      // read the frame on first frame in order to avoid blackscreen
        //         isFirstFrame = false;
        //     }

        //     TextureHandle source = resourceData.activeColorTexture;

        //     settings.afterImageMaterial.SetFloat("_BlendAmount", blendAmount);
        //     settings.afterImageMaterial.SetTexture("_HistoryTex", last_source);

        //     TextureDesc destinationDesc = source.GetDescriptor(renderGraph);
        //     destinationDesc.name = $"AfterImage-{_passName}";
        //     destinationDesc.clearBuffer = false;

        //     TextureHandle destination = renderGraph.CreateTexture(destinationDesc);


        //     RenderGraphUtils.BlitMaterialParameters temp_para = new(source, temp_source, settings.afterImageMaterial, 0);          // save the effect on the temp
        //     RenderGraphUtils.BlitMaterialParameters save_para = new(temp_source, last_source, settings.afterImageMaterial, 0);     // save the temp for the next iteration
        //     RenderGraphUtils.BlitMaterialParameters apply_para = new(temp_source, destination, settings.afterImageMaterial, 0);    // apply the effect on the current frame

        //     renderGraph.AddBlitPass(temp_para, passName: _passName);
        //     // renderGraph.AddBlitPass(save_para, passName: _passName);
        //     // renderGraph.AddBlitPass(apply_para, passName: _passName);

        //     // FrameData allows to get and set internal pipeline buffers. Here we update the CameraColorBuffer to the texture that we just wrote to in this pass. 
        //     // Because RenderGraph manages the pipeline resources and dependencies, following up passes will correctly use the right color buffer.
        //     // This optimization has some caveats. You have to be careful when the color buffer is persistent across frames and between different cameras, such as in camera stacking.
        //     // In those cases you need to make sure your texture is an RTHandle and that you properly manage the lifecycle of it.
        //     resourceData.cameraColor = destination;

        // }

        // This static method is passed as the RenderFunc delegate to the RenderGraph render pass.
        // It is used to execute draw commands.

        //

        // What the GPU execute
        static void ExecutePass(PassData data, RasterGraphContext context)
        {
            if (data.historyTexture.IsValid())
            {
                data.material.SetTexture("_HistoryTex", data.historyTexture);
                data.material.SetFloat("_BlendAmount", data.blend);

                // To blit by appling the material using the default scaleBias
                Blitter.BlitTexture(context.cmd, data.activeColor, new Vector4(1, 1, 0, 0), data.material, 0);
            }
        }

        // RecordRenderGraph is where the RenderGraph handle can be accessed, through which render passes can be added to the graph.
        // FrameData is a context container through which URP resources can be accessed and managed.
        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameContext)
        {
            VolumeStack stack = VolumeManager.instance.stack;
            AfterImageEffect effect = stack.GetComponent<AfterImageEffect>();
            if (!effect.IsActive()) return;

            float blendAmount = effect.GetIntensity();

            UniversalResourceData resourceData = frameContext.Get<UniversalResourceData>();

            if (resourceData.isActiveTargetBackBuffer) // cant read form the backbuffer (the actual buffer of the screen)
            {
                Debug.LogError("AfterImageRenderFeature requires an intermediate ColorTexture.");
                return;
            }

            // get the current screen
            TextureHandle source = resourceData.activeColorTexture;

            // grab info (description) of the screen
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

            TextureHandle historyHandle = renderGraph.ImportTexture(m_HistoryRT);

            if (isFirstFrame)
            {
                // Seed history with current frame so frame 1 isn't black
                renderGraph.AddCopyPass(source, historyHandle, passName: "AfterImage_SeedHistory");
                resourceData.cameraColor = source;
                isFirstFrame = false;
                return; // skip blend on first frame
            }

            // Create destination for this frame's output
            desc.name = "AfterImage_Destination";
            desc.clearBuffer = false;
            TextureHandle destination = renderGraph.CreateTexture(desc);

            // render pass (GPU job) 
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(_passName, out var passData))
            {
                // populating data
                passData.material = settings.afterImageMaterial;
                passData.blend = blendAmount;
                passData.activeColor = source;
                passData.historyTexture = historyHandle;

                // use those tex in readonly
                builder.UseTexture(source, AccessFlags.Read);
                builder.UseTexture(historyHandle, AccessFlags.Read);

                // set the final destination if the process
                builder.SetRenderAttachment(destination, 0);

                // assign the task
                builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
            }

            // Save destination into history for NEXT frame (before reassigning cameraColor)
            renderGraph.AddCopyPass(destination, historyHandle, passName: "AfterImage_SaveHistory");

            // Now redirect camera output
            resourceData.cameraColor = destination;
        }

        public void Dispose()
        {
            m_HistoryRT?.Release();
        }
    }
}