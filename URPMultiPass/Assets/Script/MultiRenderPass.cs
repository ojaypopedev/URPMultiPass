using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MultiRenderPass : ScriptableRenderPass
{
    private static readonly string k_RenderTag = "Blit Stages"; // Add tag for Frame Debugger
    private static readonly int ColorInvertID = Shader.PropertyToID("_ColorInvertID");
    private static readonly int OffsetTempID = Shader.PropertyToID("_OffsetTempID");
    
    private RenderTargetIdentifier currentTarget;
    private Material blitRenderMaterial;
    private Material blitOffsetImage;

    public MultiRenderPass(RenderPassEvent evt)
    {
        renderPassEvent = evt;
        blitRenderMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/InvertColor"));
        blitOffsetImage = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/OffsetImage"));
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        if (!renderingData.cameraData.postProcessEnabled) return;
        
        CommandBuffer cmd = CommandBufferPool.Get(k_RenderTag);
        Render(cmd, ref renderingData);
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    public void Setup(RenderTargetIdentifier currentTarget)
    {
        this.currentTarget = currentTarget;
    }

    void Render(CommandBuffer cmd, ref RenderingData renderingData)
    {
        CameraData cameraData = renderingData.cameraData;
        RenderTargetIdentifier source = currentTarget;
        int colorInvertTemp = ColorInvertID;
        int offsetTemp = OffsetTempID;
        
        //Scale Down
        int w = cameraData.camera.scaledPixelWidth >> 3;
        int h = cameraData.camera.scaledPixelHeight >> 3;
        
        //Temporary Textures
        cmd.GetTemporaryRT(colorInvertTemp, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
        cmd.GetTemporaryRT(offsetTemp, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
        
        //Invert Color
        cmd.Blit(source, colorInvertTemp, blitRenderMaterial, 0);
        
        //Offset Image
        cmd.Blit(colorInvertTemp, offsetTemp, blitOffsetImage, 0);
        
        //Output to Screen
        cmd.Blit(offsetTemp, source);
        
        cmd.ReleaseTemporaryRT(colorInvertTemp);
        cmd.ReleaseTemporaryRT(offsetTemp);
    }
}