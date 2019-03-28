using UnityEngine;
using System.Collections;

public class GaussianBlur : PostEffectsBase {

	public Shader gaussianBlurShader;
	public Material gaussianBlurMaterial = null;

	public Material material {  
		get {
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
	public int downSample = 2;

    /// 1st edition: just apply blur
    //	void OnRenderImage(RenderTexture src, RenderTexture dest) {
    //		if (material != null) {
    //			int rtW = src.width;
    //			int rtH = src.height;
    //			RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
    //
    //			// Render the vertical pass
    //			Graphics.Blit(src, buffer, material, 0);
    //			// Render the horizontal pass
    //			Graphics.Blit(buffer, dest, material, 1);
    //
    //			RenderTexture.ReleaseTemporary(buffer);
    //		} else {
    //			Graphics.Blit(src, dest);
    //		}
    //	} 

    /// 2nd edition: scale the render texture
    //	void OnRenderImage (RenderTexture src, RenderTexture dest) {
    //		if (material != null) {
    //			int rtW = src.width/downSample;
    //			int rtH = src.height/downSample;
    //			RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
    //			buffer.filterMode = FilterMode.Bilinear;
    //
    //			// Render the vertical pass
    //			Graphics.Blit(src, buffer, material, 0);
    //			// Render the horizontal pass
    //			Graphics.Blit(buffer, dest, material, 1);
    //
    //			RenderTexture.ReleaseTemporary(buffer);
    //		} else {
    //			Graphics.Blit(src, dest);
    //		}
    //	}

    /// 3rd edition: use iterations for larger blur

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


        //[ImageEffectOpaque]
        //  void OnRenderImage (RenderTexture src, RenderTexture dest) {
        //if (material != null) {
        //	int rtW = src.width/downSample;
        //	int rtH = src.height/downSample;

        //	RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 24);
        //	buffer0.filterMode = FilterMode.Bilinear;

        //          Graphics.SetRenderTarget(buffer0.colorBuffer, m_RenderTexture.depthBuffer);
        //	Graphics.Blit(src, gaussianBlurMaterial);


        //	//for (int i = 0; i < iterations; i++) {
        //	//	material.SetFloat("_BlurSize", 1.0f + i * blurSpread);

        //	//	RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

        //	//	// Render the vertical pass
        //	//	Graphics.Blit(buffer0, buffer1, material, 0);

        //	//	RenderTexture.ReleaseTemporary(buffer0);
        //	//	buffer0 = buffer1;
        //	//	buffer1 = RenderTexture.GetTemporary(rtW, rtH, 0);

        //	//	// Render the horizontal pass
        //	//	Graphics.Blit(buffer0, buffer1, material, 1);

        //	//	RenderTexture.ReleaseTemporary(buffer0);
        //	//	buffer0 = buffer1;
        //	//}

        //	Graphics.Blit(buffer0, dest);
        //	RenderTexture.ReleaseTemporary(buffer0);
        //} else {
        //	Graphics.Blit(src, dest);
        //}
        //}
    }
