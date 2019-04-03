using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TA;

public class TestRenderTexture : MonoBehaviour
{
    public Material CopyTextureMaterial;

    // Use this for initialization
    void Start()
    {

    }

    // 这个是所挂载的Camera渲染完成后调用的。
    void OnPostRender()
    {

        // 将主相机渲染的RenderTexture颜色内容填充给buffer1，这样之后只需要在它的基础上进行stencil遮罩，然后用相应后处理效果处理
        RenderTexture buffer1 = RenderTexture.GetTemporary(Screen.width, Screen.height, 24);

        RenderTexture.active = null;
        Graphics.Blit(buffer1, CopyTextureMaterial);
        RenderTexture.ReleaseTemporary(buffer1);
    }
}
