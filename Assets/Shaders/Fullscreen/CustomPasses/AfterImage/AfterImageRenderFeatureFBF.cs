using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;

public class AfterImageRenderFeatureFBF : ScriptableRendererFeature
{
    [SerializeField]
    private AfterImageSettings settings;
    AfterImagePass m_ScriptablePass;

    [SerializeField]
    private RenderPassEvent renderEvent = RenderPassEvent.AfterRenderingPostProcessing;

    /// <inheritdoc/>
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

        static void ExecutePass(PassData data, RasterGraphContext context)
        {
            if (data.historyTexture.IsValid())
            {
                data.material.SetTexture("_HistoryTex", data.historyTexture);
                data.material.SetFloat("_BlendAmount", data.blend);

                // Perform the blend: BlitSource (current) + _HistoryTex -> Screen
                Blitter.BlitTexture(context.cmd, data.activeColor, new Vector4(1, 1, 0, 0), data.material, 0);
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

            UniversalCameraData cameraData = frameContext.Get<UniversalCameraData>();
            UniversalResourceData resourceData = frameContext.Get<UniversalResourceData>();

            if (resourceData.isActiveTargetBackBuffer)
            {
                Debug.LogError($"Skipping render pass. AfterImageRenderFeature requires an intermediate ColorTexture, we can't use the BackBuffer as a texture input.");
                return;
            }

            // Request access to history (persistent textures)
            cameraData.historyManager.RequestAccess<RawColorHistory>();
            RawColorHistory history = cameraData.historyManager.GetHistoryForRead<RawColorHistory>();

            TextureHandle source = resourceData.activeColorTexture;

            // Create the destination texture
            var destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = "FBFetchDestTexture";
            destinationDesc.clearBuffer = false;

            TextureHandle destination = renderGraph.CreateTexture(destinationDesc);

            // Setup the pass
            using (var builder = renderGraph.AddRasterRenderPass<PassData>(_passName, out var passData))
            {
                // store all the passData
                passData.material = settings.afterImageMaterial;
                passData.blend = blendAmount;
                passData.activeColor = source;

                // Import the history texture if it exists
                RTHandle historyRT = history?.GetPreviousTexture(0);
                if (historyRT != null)
                {
                    passData.historyTexture = renderGraph.ImportTexture(historyRT);
                    builder.UseTexture(passData.historyTexture, AccessFlags.Read);
                }

                // We declare the src as input attachment. This is required for framebuffer fetch. 
                builder.SetInputAttachment(source, 0, AccessFlags.Read);

                // Setup as a render target via UseTextureFragment, which is the equivalent of using the old cmd.SetRenderTarget
                builder.SetRenderAttachment(destination, 0);

                // We disable culling for this pass for the demonstrative purpose of this sample, as normally this pass would be culled,
                // since the destination texture is not used anywhere else
                builder.AllowPassCulling(false);

                // Assign the ExecutePass function to the render pass delegate, which will be called by the render graph when executing the pass
                builder.SetRenderFunc((PassData data, RasterGraphContext context) => ExecutePass(data, context));
            }

            // Copy back the FBF output to the camera color to easily see the result in the game view
            // This copy pass also uses FBF under the hood. All the passes should be merged this way and the destination attachment should be memoryless (no load/store of memory).
            renderGraph.AddCopyPass(destination, source, passName);
        }
    }
}