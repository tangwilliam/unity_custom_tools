using UnityEngine;
using System.Collections;

public class SimpleEffect: PostEffectsBase {

	public Shader gaussianBlurShader;
	public Material gaussianBlurMaterial = null;

	public Material material {  
		get {
			gaussianBlurMaterial = CheckShaderAndCreateMaterial(gaussianBlurShader, gaussianBlurMaterial);
			return gaussianBlurMaterial;
		}  
	}

    private RenderTexture m_RenderTexture;
    private RenderTexture m_BloomRT0;
    private Camera m_Camera;

    protected void Start()
    {
        Debug.Log("Gaussian Blur start");
        base.Start();

        RenderTextureFormat format = RenderTextureFormat.Default;
        if (SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.ARGBHalf))
        {
            format = RenderTextureFormat.ARGBHalf;
        }
        m_RenderTexture = new RenderTexture(Screen.width, Screen.height, 24, format);
        m_RenderTexture.Create();

        m_Camera = GetComponent<Camera>();
        m_Camera.targetTexture = m_RenderTexture;

        m_BloomRT0 = new RenderTexture(m_RenderTexture.width, m_RenderTexture.height, 0, format);
        m_BloomRT0.Create();	//Ensure these two RT have the same size
    }

    public void OnPostRender()
    {
        //postProcessingMaterial.SetPass(1);

        m_BloomRT0.DiscardContents();
        Graphics.SetRenderTarget(m_BloomRT0);
        GL.Clear(true, true, new Color(0, 0, 0, 0));    // clear the full RT
                                                        // *KEY POINT*: Draw with the camera's depth buffer
        Graphics.SetRenderTarget(m_BloomRT0.colorBuffer, m_RenderTexture.depthBuffer);
        Graphics.Blit(m_RenderTexture, gaussianBlurMaterial);
    }


 
    }
