using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

namespace Rendering {

public class ScreenSpaceOutline : ScriptableRendererFeature {
    [System.Serializable]
    class ViewSpaceNormalsTextureSettings {
        [Header("General Texture Settings")]
        public RenderTextureFormat colorFormat;
        public int depthBufferBits = 16;
        public Color backgroundColor = Color.black;
    }


    class ViewspaceNormalsTexturePass : ScriptableRenderPass {
        private readonly RenderTargetHandle normals;
        private readonly Material normalsMaterial;
        private readonly List<ShaderTagId> shaderTagIdList;
        private ViewSpaceNormalsTextureSettings settings;
        private FilteringSettings filteringSettings;

        public ViewspaceNormalsTexturePass(RenderPassEvent renderPassEvent, ViewSpaceNormalsTextureSettings settings,
                                           Material normalsMaterial, LayerMask outlinesLayerMask) {
            this.renderPassEvent = renderPassEvent;
            this.settings = settings;
            this.normalsMaterial = normalsMaterial;
            filteringSettings = new FilteringSettings(
                RenderQueueRange.opaque,
                outlinesLayerMask);

            shaderTagIdList = new List<ShaderTagId> {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightweightForward"),
                new ShaderTagId("SRPDefaultUnlit")
            };

            normals.Init("_ScreenViewSpaceNormals");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor) {
            RenderTextureDescriptor normalsTextureDescriptor = cameraTextureDescriptor;

            normalsTextureDescriptor.colorFormat = settings.colorFormat;
            normalsTextureDescriptor.depthBufferBits = settings.depthBufferBits;

            cmd.GetTemporaryRT(normals.id, normalsTextureDescriptor, FilterMode.Point);
            ConfigureTarget(normals.Identifier());
            ConfigureClear(ClearFlag.All, settings.backgroundColor);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            if (!normalsMaterial)
                return;
            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, new ProfilingSampler("SceneViewSpaceNormalsTextureCreation"))) {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                DrawingSettings drawingSettings = CreateDrawingSettings(
                                                      shaderTagIdList, ref renderingData,
                                                      renderingData.cameraData.defaultOpaqueSortFlags
                                                  );
                drawingSettings.overrideMaterial = normalsMaterial;
                context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(normals.id);
        }
    }

    class ScreenSpaceOutlineRenderPass : ScriptableRenderPass {
        private readonly Material screenSpaceOutlineMaterial;

        RenderTargetIdentifier cameraColorTarget;
        RenderTargetIdentifier temporaryBuffer;
        int temporaryBufferID = Shader.PropertyToID("_TemporaryBuffer");

        public ScreenSpaceOutlineRenderPass(RenderPassEvent renderPassEvent, Material outlineMaterial) {
            this.renderPassEvent = renderPassEvent;
            this.screenSpaceOutlineMaterial = outlineMaterial;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
            RenderTextureDescriptor temporaryTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            temporaryTargetDescriptor.depthBufferBits = 0;
            cmd.GetTemporaryRT(temporaryBufferID, temporaryTargetDescriptor, FilterMode.Bilinear);
            temporaryBuffer = new RenderTargetIdentifier(temporaryBufferID);

            cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            if (!screenSpaceOutlineMaterial)
                return;

            CommandBuffer cmd = CommandBufferPool.Get();

            using (new ProfilingScope(cmd, new ProfilingSampler("ScreenSpaceOutlines"))) {
                Blit(cmd, cameraColorTarget, temporaryBuffer);
                Blit(cmd, temporaryBuffer, cameraColorTarget, screenSpaceOutlineMaterial);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(temporaryBufferID);
        }
    }

    [SerializeField] RenderPassEvent renderPassEvent;
    [SerializeField] LayerMask outlinesLayerMask;
    [SerializeField] ViewSpaceNormalsTextureSettings viewspaceNormalsTextureSettings;
    [SerializeField] Material viewspaceNormalsBlitMaterial;

    [SerializeField] Material screenSpaceOutlineMaterial;

    ViewspaceNormalsTexturePass viewspaceNormalTexturePass;
    ScreenSpaceOutlineRenderPass screenSpaceOutlinePass;

    /// <inheritdoc/>
    public override void Create() {
        viewspaceNormalTexturePass = new ViewspaceNormalsTexturePass(renderPassEvent, viewspaceNormalsTextureSettings, viewspaceNormalsBlitMaterial, outlinesLayerMask);
        screenSpaceOutlinePass = new ScreenSpaceOutlineRenderPass(renderPassEvent, screenSpaceOutlineMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass(viewspaceNormalTexturePass);
        renderer.EnqueuePass(screenSpaceOutlinePass);
    }
}


}
