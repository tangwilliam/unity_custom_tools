using UnityEngine;
using System.Collections;
using TA;

public class GaussianBlurWtihStencil : PostEffectsBase
{

    public Shader gaussianBlurShader;
    private Material gaussianBlurMaterial = null;
    public Material CopyTextureMaterial;


    public Material material
    {
        get
        {
            gaussianBlurMaterial = CheckShaderAndCreateMaterial(gaussianBlurShader, gaussianBlurMaterial);
            return gaussianBlurMaterial;
        }
    }

    // Blur iterations - larger number means more blur.
    [Range(0, 4)]
    public int iterations = 3;

    // Blur spread for each iteration - larger value means more blur
    [Range(0.2f, 3.0f)]
    public float blurSpread = 0.6f;

    [Range(1, 8)]
    public int downSample = 1;
 

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material != null)
        {
            int rtW = src.width;
            int rtH = src.height;

            // 将主相机渲染的RenderTexture颜色内容填充给buffer1，这样之后只需要在它的基础上进行stencil遮罩，然后用相应后处理效果处理
            RenderTexture bufferFinal = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
            PostEffectsUtil.CustomBlit(src, bufferFinal.colorBuffer, src.depthBuffer, CopyTextureMaterial, 0);

            
            RenderTexture buffer0 = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
            
            //test
            Graphics.SetRenderTarget(buffer0);
            GL.Clear(true, true, Color.black);

            PostEffectsUtil.CustomBlit(src, buffer0.colorBuffer, src.depthBuffer, material, 0);

            //--------------------- 
            // loop

            // set property
            material.SetFloat("_BlurSize", 1.0f + 0 * blurSpread);

            // vertical pass
            RenderTexture buffer1 = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);

            //test
            Graphics.SetRenderTarget(buffer1);
            GL.Clear(true, true, Color.black);

            PostEffectsUtil.CustomBlit(buffer0, buffer1.colorBuffer, src.depthBuffer, material, 0);
            RenderTexture.ReleaseTemporary(buffer0);

            RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 24);

            //test
            Graphics.SetRenderTarget(buffer2);
            GL.Clear(true, true, Color.black);

            // Render the horizontal pass
            PostEffectsUtil.CustomBlit(buffer1, buffer2.colorBuffer, src.depthBuffer, material, 1);
            RenderTexture.ReleaseTemporary(buffer1);

            //--------------------- 
            // loop

            // set property
            material.SetFloat("_BlurSize", 1.0f + 1 * blurSpread);

            // vertical pass
            RenderTexture buffer3 = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);

            //test
            Graphics.SetRenderTarget(buffer3);
            GL.Clear(true, true, Color.black);

            PostEffectsUtil.CustomBlit(buffer2, buffer3.colorBuffer, src.depthBuffer, material, 0);
            RenderTexture.ReleaseTemporary(buffer2);
            
            RenderTexture buffer4 = RenderTexture.GetTemporary(rtW, rtH, 24);

            //test
            Graphics.SetRenderTarget(buffer4);
            GL.Clear(true, true, Color.black);

            // Render the horizontal pass
            PostEffectsUtil.CustomBlit(buffer3, buffer4.colorBuffer, src.depthBuffer, material, 1);
            RenderTexture.ReleaseTemporary(buffer3);
           
            //--------------------------------
            // Render to final buffer
            PostEffectsUtil.CustomBlit(buffer4, bufferFinal.colorBuffer, src.depthBuffer, material, 1);
            RenderTexture.ReleaseTemporary(buffer4);

            Graphics.Blit(bufferFinal, dest);

            RenderTexture.ReleaseTemporary(bufferFinal);
            

        }
        else
        {
            Graphics.Blit(src, dest);
        }
    }
}
