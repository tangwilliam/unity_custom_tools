using UnityEngine;
using System.Collections;

public class SimpleEffect: PostEffectsBase {

    public Shader postEffectShader;
    private Material gaussianBlurMaterial = null;
    private RenderTexture buffer0;

    public Material material
    {
        get
        {
            gaussianBlurMaterial = CheckShaderAndCreateMaterial(postEffectShader, gaussianBlurMaterial);
            return gaussianBlurMaterial;
        }
    }


    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {
            int rtW = src.width ;
            int rtH = src.height ;
            
            buffer0 = RenderTexture.GetTemporary(rtW, rtH, 24);
            buffer0.filterMode = FilterMode.Bilinear;
            //Graphics.SetRenderTarget(buffer0.colorBuffer, src.depthBuffer);

            Graphics.Blit(src, buffer0, material,0);

            Graphics.Blit(buffer0, dest);
            RenderTexture.ReleaseTemporary(buffer0);
        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }



}
