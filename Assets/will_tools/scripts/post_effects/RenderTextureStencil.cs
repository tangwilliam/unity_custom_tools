using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTextureStencil : MonoBehaviour {

    public Material postProcessingMaterial;

    private RenderTexture renderTexture;
    private RenderTexture bloomRT0;



    // Use this for initialization
    void Start () {

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
        postProcessingMaterial.SetPass(1);

        bloomRT0.DiscardContents();
        Graphics.SetRenderTarget(bloomRT0);
        GL.Clear(true, true, new Color(0, 0, 0, 0));    // clear the full RT
                                                        // *KEY POINT*: Draw with the camera's depth buffer
        Graphics.SetRenderTarget(bloomRT0.colorBuffer, renderTexture.depthBuffer);
        Graphics.Blit(renderTexture, postProcessingMaterial, 1);
    }
}
