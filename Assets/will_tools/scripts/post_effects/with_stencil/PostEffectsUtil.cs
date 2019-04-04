using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TA
{
    public static class PostEffectsUtil
    {

        static public void CustomBlit(RenderTexture src, RenderBuffer destCOLOR, RenderBuffer destDEPTH, Material MRTMat, int pass)
        {
            MRTMat.mainTexture = src;
            Graphics.SetRenderTarget(destCOLOR, destDEPTH);
            RenderQuad(MRTMat, pass);
        }

        /// <summary>
        /// 将两张RenderTexture进行相加，要注意提供的Shader中需要包含"_MainTex"和"_SecondRT"两个属性。
        /// </summary>
        /// <param name="MainTex"></param>
        /// <param name="SecondRT"></param>
        /// <param name="TwoTexsMat"></param>
        /// <param name="pass"></param>
        static public void OverlayRenderTextures( RenderTexture MainTex, RenderTexture SecondRT, RenderBuffer DestCOLOR, RenderBuffer DestDEPTH, Material TwoTexsMat, int pass)
        {
            TwoTexsMat.mainTexture = MainTex;
            TwoTexsMat.SetTexture("_SecondRT", SecondRT);
            Graphics.SetRenderTarget(DestCOLOR, DestDEPTH);
            RenderQuad(TwoTexsMat, pass);
        }


        static public void RenderQuad(Material MRTMat, int pass)
        {

            GL.PushMatrix();
            GL.LoadOrtho();
            MRTMat.SetPass(pass);
            GL.Begin(GL.QUADS);
            GL.TexCoord2(0.0f, 1.0f); GL.Vertex3(0.0f, 1.0f, 0.1f);
            GL.TexCoord2(1.0f, 1.0f); GL.Vertex3(1.0f, 1.0f, 0.1f);
            GL.TexCoord2(1.0f, 0.0f); GL.Vertex3(1.0f, 0.0f, 0.1f);
            GL.TexCoord2(0.0f, 0.0f); GL.Vertex3(0.0f, 0.0f, 0.1f);
            GL.End();
            GL.PopMatrix();
        }


    }
}


