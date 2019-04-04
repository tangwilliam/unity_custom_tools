using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TA;

/// <summary>
/// 本方案实现的效果是：场景中stencil设置为2的区域不进行后处理处理，2以外的进行后处理。
/// 使用方法：挂在MainCamera上
/// 使用注意：stencil区域中如果有透明区域可能出现黑边的原因是其边缘存在渐变透明，可以通过增大clip()的值减小黑边。同时需要去除Cm
/// 常规方式的问题是：在Blit(CameraRenderTexture,PostprocessMaterial)时，读取的stencilBuffer已经被清除掉。无法实现将stencilBuffer传递给多个RenderTexture.
/// 同时，本方案使用了自定义的 CustomBlit() 方法，解决stencil丢失的问题，同时存在对同一张RT不Clear就再次叠加渲染出现的警告Graphics.Blit Tiled GPU perf. warning: RenderTexture color surface was not cleared/discarded: ( That is really only a performance warning. Because mobile devices commonly use tile based rendering, you should not expect the RenderTexture to keep its contents of the last frame. (While on desktop hardware that is the case.) So if you don't clear the RenderTexture between frames, Unity will copy the contents back and forward to match the expected behaviour on desktop hardware. Which, if you don't need that, is a waste of performance.）So, clearing a RenderTexture takes time on desktop hardware.Not clearing it generally takes time on mobile hardware.）
/// 如果要避免这个警告和性能的损耗，可以使用 PostEffectWithStencilTwoTextures.cs的方案。但那个方案也带来了许多新的render操作。在三星C8（联发科）上测试本方案更快。但是联发科虽然是PowerVR授权，但不确定是不是TBR模式，还需要在苹果和高通芯片的手机上测试。
/// </summary>
public class PostEffectsWithStencil : MonoBehaviour {

    public Material PostprocessMaterial;
    public Material CopyTextureMaterial;

 
    
    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {

        // 将主相机渲染的RenderTexture颜色内容填充给buffer1，这样之后只需要在它的基础上进行stencil遮罩，然后用相应后处理效果处理
        RenderTexture buffer1 = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);

        PostEffectsUtil.CustomBlit(src, buffer1.colorBuffer, src.depthBuffer, CopyTextureMaterial, 0);

        //----------------------------------------------------------------------
        // 只绘制Stencil通过的部分

        RenderTexture buffer0 = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);
        PostEffectsUtil.CustomBlit(src, buffer0.colorBuffer, src.depthBuffer, PostprocessMaterial, 1);

        
        PostEffectsUtil.CustomBlit(buffer0, buffer1.colorBuffer, src.depthBuffer, PostprocessMaterial, 2);
        RenderTexture.ReleaseTemporary(buffer0);

        // 只绘制Stencil通过的部分
        //----------------------------------------------------------------------

        // 渲染到屏幕
        Graphics.Blit(buffer1,dest);
        RenderTexture.ReleaseTemporary(buffer1);


    }

}
