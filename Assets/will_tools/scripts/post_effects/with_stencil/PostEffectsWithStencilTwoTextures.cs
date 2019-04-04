using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TA;

/// <summary>
/// 本方案实现的效果是：场景中stencil设置为2的区域不进行后处理处理，2以外的进行后处理。最后将两个区域的图叠加起来。
/// 使用方法：挂在MainCamera上
/// 使用注意：stencil区域中如果有透明区域可能出现黑边的原因是其边缘存在渐变透明，可以通过增大clip()的值减小黑边
/// 常规方式的问题是：在Blit(CameraRenderTexture,PostprocessMaterial)时，读取的stencilBuffer已经被清除掉。无法实现将stencilBuffer传递给多个RenderTexture.
/// 同时，本方案使用了自定义的 CustomBlit() 和 OverlayRenderTextures() 方法，前者是解决stencil丢失的问题，后者是解决对同一张RT不Clear就再次叠加渲染出现的警告Graphics.Blit Tiled GPU perf. warning: RenderTexture color surface was not cleared/discarded: ( That is really only a performance warning. Because mobile devices commonly use tile based rendering, you should not expect the RenderTexture to keep its contents of the last frame. (While on desktop hardware that is the case.) So if you don't clear the RenderTexture between frames, Unity will copy the contents back and forward to match the expected behaviour on desktop hardware. Which, if you don't need that, is a waste of performance.）So, clearing a RenderTexture takes time on desktop hardware.Not clearing it generally takes time on mobile hardware.）
/// 本方案增加了许多Clear的操作，是因为必须保证那些RenderTexture没有被渲染的部分都是黑色的，因为最后合并的时候是两张贴图的颜色相加。
/// </summary>
public class PostEffectsWithStencilTwoTextures : MonoBehaviour {

    public Material PostprocessMaterial;
    public Material CopyTextureMaterial;
    public Material OverlayRTMaterial;


   void OnRenderImage(RenderTexture src, RenderTexture dest)
    {

        RenderTexture BufferForInStencilArea = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);

        // 需要Clear为黑色。GetTemporay()是将这张RenderTexture重复利用，所以会保留上一帧渲染的结果。
        Graphics.SetRenderTarget(BufferForInStencilArea);
        GL.Clear(true, true, Color.black);

        // 将主相机渲染的RenderTexture中Stencil区域内的内容填充给BufferForInStencilArea，用于之后与stencil区域外的RenderTexture叠加
        PostEffectsUtil.CustomBlit(src, BufferForInStencilArea.colorBuffer, src.depthBuffer, PostprocessMaterial, 0);

        //-----------------------------------------------
        // 只绘制Stencil通过的部分

        RenderTexture buffer0 = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);

        // 将buffer Clear为黑色。GetTemporay()是将这张RenderTexture重复利用，所以会保留上一帧渲染的结果。
        Graphics.SetRenderTarget(buffer0);
        GL.Clear(true, true, Color.black);

        PostEffectsUtil.CustomBlit(src, buffer0.colorBuffer, src.depthBuffer, PostprocessMaterial, 1);

        RenderTexture buffer1 = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);

        // 将buffer Clear为黑色。GetTemporay()是将这张RenderTexture重复利用，所以会保留上一帧渲染的结果。
        Graphics.SetRenderTarget(buffer1);
        GL.Clear(true, true, Color.black);

        PostEffectsUtil.CustomBlit(buffer0, buffer1.colorBuffer, src.depthBuffer, PostprocessMaterial, 2);
        RenderTexture.ReleaseTemporary(buffer0);

        // 只绘制Stencil通过的部分
        //------------------------------------------------

        // 将两张图合并
        RenderTexture bufferFinal = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
        PostEffectsUtil.OverlayRenderTextures(BufferForInStencilArea, buffer1, bufferFinal.colorBuffer, bufferFinal.depthBuffer, OverlayRTMaterial, 0);

        RenderTexture.ReleaseTemporary(BufferForInStencilArea);
        RenderTexture.ReleaseTemporary(buffer1);

        // 渲染到屏幕
        Graphics.Blit(bufferFinal,dest, CopyTextureMaterial);
        RenderTexture.ReleaseTemporary(bufferFinal);
        


    }

}
