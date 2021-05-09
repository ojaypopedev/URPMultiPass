using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MultiRenderComputeFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class ComputeFeatureSettings
    {
       public RenderPassEvent _event;
       public ComputeShader _shader;
    }

   [SerializeField] public ComputeFeatureSettings settings = new ComputeFeatureSettings();

    private MultiRenderComputePass blitRenderPass;
    
    public override void Create()
    {
        blitRenderPass = new MultiRenderComputePass(settings);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        blitRenderPass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(blitRenderPass);

        
        
    }
}