using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PixelFeature : ScriptableRendererFeature {
    [System.Serializable]
    public class CustomPassSettings {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    [SerializeField] private CustomPassSettings settings;
    private PixelShaderPass customPass;
    public override void Create() {
        customPass = new PixelShaderPass(settings);
    }


    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        renderer.EnqueuePass(customPass);
    }

    public class PixelShaderPass : ScriptableRenderPass {
        private RenderTargetIdentifier colorBuffer, pixelBuffer;
        private int pixelBufferID = Shader.PropertyToID("_DetectiveBuffer");
        private Material material;
        private GameObject[] glowObjects;

        public PixelShaderPass(CustomPassSettings settings) {
            renderPassEvent = settings.renderPassEvent;
            material = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/PixelShader")); // Updated to use Shader.Find
        }


        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
            colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.height = 1920;
            descriptor.width = 1080;
            cmd.GetTemporaryRT(pixelBufferID, descriptor, FilterMode.Point);
            pixelBuffer = new RenderTargetIdentifier(pixelBufferID);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            CommandBuffer cmd = CommandBufferPool.Get();

            //responsible for drawing the background colour from the shader
            cmd.Blit(colorBuffer, pixelBuffer, material);
            cmd.Blit(pixelBuffer, colorBuffer);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }




        public override void OnCameraCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(pixelBufferID);
        }
    }
}
