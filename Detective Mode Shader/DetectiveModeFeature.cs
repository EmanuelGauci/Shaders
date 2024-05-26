using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine;

public class DetectiveModeFeature : ScriptableRendererFeature {
    [System.Serializable]
    public class CustomPassSettings {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    [SerializeField] private CustomPassSettings settings;
    private DetectiveModePass customPass;
    private GameObject[] glowObjects;

    public override void Create() {
        customPass = new DetectiveModePass(settings);
    }

    public void SetGlowObjects(GameObject[] objects) {
        glowObjects = objects;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
        customPass.SetGlowObjects(glowObjects); // Pass glowObjects to the DetectiveModePass
        renderer.EnqueuePass(customPass);
    }

    public class DetectiveModePass : ScriptableRenderPass {
        private RenderTargetIdentifier colorBuffer, detectiveBuffer;
        private int detectiveBufferID = Shader.PropertyToID("_DetectiveBuffer");
        private Material material;
        private GameObject[] glowObjects;

        public DetectiveModePass(CustomPassSettings settings) {
            renderPassEvent = settings.renderPassEvent;
            material = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/DetectiveMode")); // Updated to use Shader.Find
        }

        public void SetGlowObjects(GameObject[] objects) {
            glowObjects = objects;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData) {
            colorBuffer = renderingData.cameraData.renderer.cameraColorTarget;
            RenderTextureDescriptor descriptor = renderingData.cameraData.cameraTargetDescriptor;
            descriptor.height = 1920;
            descriptor.width = 1080;
            cmd.GetTemporaryRT(detectiveBufferID, descriptor, FilterMode.Point);
            detectiveBuffer = new RenderTargetIdentifier(detectiveBufferID);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) {
            CommandBuffer cmd = CommandBufferPool.Get();
            
            //responsible for drawing the background colour from the shader
            cmd.Blit(colorBuffer, detectiveBuffer, material);
            cmd.Blit(detectiveBuffer, colorBuffer);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }




        public override void OnCameraCleanup(CommandBuffer cmd) {
            cmd.ReleaseTemporaryRT(detectiveBufferID);
        }
    }
}
