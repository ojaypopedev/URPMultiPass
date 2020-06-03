using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MultiRenderFeature : ScriptableRendererFeature
{
    private MultiRenderPass blitRenderPass;
    
    public override void Create()
    {
        blitRenderPass = new MultiRenderPass(RenderPassEvent.BeforeRenderingPostProcessing);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        blitRenderPass.Setup(renderer.cameraColorTarget);
        renderer.EnqueuePass(blitRenderPass);
    }
}