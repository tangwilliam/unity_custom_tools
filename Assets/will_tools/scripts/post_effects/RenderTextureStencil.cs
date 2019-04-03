using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureStencil : PostEffectsBase {

    public RenderTexture resultRT;

    private RenderTexture renderTexture;
    private RenderTexture bloomRT0;

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

    // Use this for initialization
    new

    // Use this for initialization
    void Start()
    {

        //base.Start();

        RenderTextureFormat format = RenderTextureFormat.Default;
        if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
        {
            format = RenderTextureFormat.ARGBHalf;
        }
        renderTexture = new RenderTexture(Screen.width, Screen.height, 24, format);
        renderTexture.Create();
        GetComponent<Camera>().targetTexture = renderTexture;

        bloomRT0 = new RenderTexture(renderTexture.width, renderTexture.height, 0, format);
        bloomRT0.Create();  //Ensure these two RT have the same size
    }



    public void OnPostRender()
    {
        //material.SetPass(1);

        bloomRT0.DiscardContents();
        Graphics.SetRenderTarget(bloomRT0);
        GL.Clear(true, true, new Color(0, 0, 0, 0));    // clear the full RT
                                                        // *KEY POINT*: Draw with the camera's depth buffer
        Graphics.SetRenderTarget(bloomRT0.colorBuffer, renderTexture.depthBuffer);
        Graphics.Blit(renderTexture, resultRT, material);
    }
}
