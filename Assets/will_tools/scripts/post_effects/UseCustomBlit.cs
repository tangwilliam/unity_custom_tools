using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TA;

public class UseCustomBlit : MonoBehaviour
{

    public Material PostprocessMaterial;
    public Material SimpleRender;

    public RenderTexture CameraRenderTexture;
    public RenderTexture BufferForFinalBlit;
    public RenderTexture BufferForMiddleBlit;

    [Range(0.2f, 3.0f)]
    public float blurSpread = 0.6f;

    // 常规方式的问题是：在Blit(CameraRenderTexture,PostprocessMaterial)时，读取的stencilBuffer已经被清除掉。
    // 本方案的思路是：通过SetRenderTarget将渲染目标的colorBuffer设置为一张用来最后显示画面的颜色图，将
    public void Start()
    {
        CameraRenderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        BufferForFinalBlit = new RenderTexture(Screen.width, Screen.height, 24);
        BufferForMiddleBlit = new RenderTexture(Screen.width, Screen.height, 24);

        // 让主相机渲染目标设置为一张RenderTexture
        Camera.main.targetTexture = CameraRenderTexture;
    }

    // 这个是所挂载的Camera渲染完成后调用的。
    void OnPostRender()
    {
        // 这里将BufferForFinalBlit设置为Active的目的是使用GL.Clear()来清除之前的depth(包含stencil)和color
        Graphics.SetRenderTarget(BufferForMiddleBlit);
        // Clear the current render buffer.This clears the screen or the active RenderTexture you are drawing into.
        // Clear(bool clearDepth, bool clearColor, Color backgroundColor, float depth = 1.0f);
        GL.Clear(true, true, Color.black);


        //Graphics.SetRenderTarget(BufferForFinalBlit.colorBuffer, CameraRenderTexture.depthBuffer);

        // 这个是正常颜色绘制，如果没有后处理，这个就是最终效果
        Graphics.Blit(CameraRenderTexture, SimpleRender);


        PostEffectsUtil.CustomBlit(CameraRenderTexture, BufferForMiddleBlit.colorBuffer, CameraRenderTexture.depthBuffer, SimpleRender, 0);

        // 这个只绘制Stencil通过的部分
        for (int i = 0; i < 3; i++)
        {
            PostprocessMaterial.SetFloat("_BlurSize", 1.0f + i * blurSpread);

            RenderTexture buffer1 = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
            //CustomBlit(CameraRenderTexture, buffer1.colorBuffer, CameraRenderTexture.depthBuffer, SimpleRender, 0);

            // Render the vertical pass
            PostEffectsUtil.CustomBlit(BufferForMiddleBlit, buffer1.colorBuffer,CameraRenderTexture.depthBuffer, PostprocessMaterial, 0);

            //RenderTexture.ReleaseTemporary(buffer0);
            BufferForMiddleBlit = buffer1;
            buffer1 = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
            PostEffectsUtil.CustomBlit(CameraRenderTexture, buffer1.colorBuffer, CameraRenderTexture.depthBuffer, SimpleRender, 0);

            //Graphics.SetRenderTarget(buffer0.colorBuffer, src.depthBuffer);

            // Render the horizontal pass
            PostEffectsUtil.CustomBlit(BufferForMiddleBlit, buffer1.colorBuffer, CameraRenderTexture.depthBuffer, PostprocessMaterial, 1);

            //RenderTexture.ReleaseTemporary(buffer0);
            BufferForMiddleBlit = buffer1;
            //RenderTexture.ReleaseTemporary(buffer1);
        }


        // All rendering goes into the active RenderTexture. If the active RenderTexture is null everything is rendered in the main window.

        RenderTexture.active = null;
        Graphics.Blit(BufferForMiddleBlit, SimpleRender);
    }

    
}
