using UnityEngine;
using System.Collections;
using TA;

public class BloomWithStencil : PostEffectsBase {

    public Shader bloomShader;
	private Material bloomMaterial = null;
	public Material material {  
		get {
			bloomMaterial = CheckShaderAndCreateMaterial(bloomShader, bloomMaterial);
			return bloomMaterial;
		}  
	}

	// Blur iterations - larger number means more blur.
	[Range(0, 4)]
	public int iterations = 3;
	
	// Blur spread for each iteration - larger value means more blur
	[Range(0.2f, 3.0f)]
	public float blurSpread = 0.6f;


	[Range(0.0f, 4.0f)]
	public float luminanceThreshold = 0.6f;

	void OnRenderImage (RenderTexture src, RenderTexture dest) {
		if (material != null) {

            int rtW = src.width;
            int rtH = src.height;

            // 提取画面中的高亮部分
            material.SetFloat("_LuminanceThreshold", luminanceThreshold);
            RenderTexture buffer0 = RenderTexture.GetTemporary(rtW, rtH, 24);
            PostEffectsUtil.CustomBlit(src, buffer0.colorBuffer, src.depthBuffer, material, 0);

            //--------------------- 
            // 模糊1 （因为一直没有理清楚冯乐乐对于RenderTexture的 ReleaseTemporary操作及生疏了C sharp，所以这里没用for循环）

            // set property
            material.SetFloat("_BlurSize", 1.0f + 1 * blurSpread);

            // vertical pass
            RenderTexture buffer1 = RenderTexture.GetTemporary(rtW, rtH, 24);

            //这是模糊开始的第一个Pass，需要将该buffer进行clear，否则stencil区域内的颜色会因为跨界采样被带入到非stencil区域。
            Graphics.SetRenderTarget(buffer1);
            GL.Clear(true, true, Color.black);

            PostEffectsUtil.CustomBlit(buffer0, buffer1.colorBuffer, src.depthBuffer, material, 1);
            RenderTexture.ReleaseTemporary(buffer0);

            RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 24);
            // Render the horizontal pass
            PostEffectsUtil.CustomBlit(buffer1, buffer2.colorBuffer, src.depthBuffer, material, 2);
            RenderTexture.ReleaseTemporary(buffer1);

            //--------------------- 
            // 模糊2

            // set property
            material.SetFloat("_BlurSize", 1.0f + 2 * blurSpread);

            // vertical pass
            RenderTexture buffer3 = RenderTexture.GetTemporary(rtW, rtH, 24);
            PostEffectsUtil.CustomBlit(buffer2, buffer3.colorBuffer, src.depthBuffer, material, 1);
            RenderTexture.ReleaseTemporary(buffer2);

            RenderTexture buffer4 = RenderTexture.GetTemporary(rtW, rtH, 24);
            // Render the horizontal pass
            PostEffectsUtil.CustomBlit(buffer3, buffer4.colorBuffer, src.depthBuffer, material, 2);
            RenderTexture.ReleaseTemporary(buffer3);

            //----------------------
            // 与原图合并高亮效果
            material.SetTexture ("_Bloom", buffer4);  
			Graphics.Blit (src, dest, material, 3);  
			RenderTexture.ReleaseTemporary(buffer4);

		} else {
			Graphics.Blit(src, dest);
		}
	}
}
