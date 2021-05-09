using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class MultiRenderComputePass : ScriptableRenderPass
{
    private static readonly string k_RenderTag = "Blit Stages"; // Add tag for Frame Debugger
    private static readonly int ColorInvertID = Shader.PropertyToID("_ColorInvertID");
    private static readonly int OffsetTempID = Shader.PropertyToID("_OffsetTempID");
   
    
    private RenderTargetIdentifier currentTarget;
    private Material blitRenderMaterial;
    private Material blitOffsetImage;

    Vector2Int groupSize;

    MultiRenderComputeFeature.ComputeFeatureSettings _settings;

    public MultiRenderComputePass(MultiRenderComputeFeature.ComputeFeatureSettings settings)
    {
        _settings = settings;
        renderPassEvent = _settings._event;
        blitRenderMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/InvertColor"));
        blitOffsetImage = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/OffsetImage"));
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        //if (!renderingData.cameraData.postProcessEnabled) return;
        
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

        //GEt the rendering datafrom the camera
        CameraData cameraData = renderingData.cameraData;    
        RenderTargetIdentifier source = currentTarget;
        
        int output = 0;
        
        //Get Camera Width and Height
        int w = cameraData.camera.scaledPixelWidth;
        int h = cameraData.camera.scaledPixelHeight;

        //Temporary Textures
        cmd.GetTemporaryRT(output, w, h, 0, FilterMode.Point, RenderTextureFormat.Default,
            RenderTextureReadWrite.sRGB, 1, true);       

        //Invert Color
        cmd.Blit(source, output);

        //COMPUTE CODE GOES HERE?

        int handle = _settings._shader.FindKernel("CSMain");
        uint x, y = 0;
        _settings._shader.GetKernelThreadGroupSizes(handle, out x, out y, out _);
       

        if(groupSize ==null)
        {
            groupSize = new Vector2Int();
        }

         //Probaly could be better 
        groupSize = Vector2Int.RoundToInt(new Vector2((float)w / (float)x, (float)h / (float)y));

     
        cmd.SetComputeTextureParam(_settings._shader, handle, "source", source);
        cmd.SetComputeTextureParam(_settings._shader, handle, "output", output);
        cmd.DispatchCompute(_settings._shader, handle, groupSize.x, groupSize.y, 1);

        //COMPUTE CODE GOES HERE?

        //Output to Screen
        cmd.Blit(output, source);        
        cmd.ReleaseTemporaryRT(output);
       
    }
}