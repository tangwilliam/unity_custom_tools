Shader "Custom/post_add_color" {
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		// No culling or depth
		Cull Off  ZWrite Off ZTest Always

		// Pass 0
		Pass
		{
			Stencil{
				Ref 2
				Comp Equal
			}
			CGPROGRAM
	#pragma vertex vert_img
	#pragma fragment frag
	#pragma fragmentoption ARB_precision_hint_fastest
	#include "UnityCG.cginc"

			sampler2D _MainTex;
			half4 frag(v2f_img i) : COLOR
			{
				return tex2D(_MainTex, i.uv);
			}

			ENDCG
		}

	// Pass 1
	Pass
	{
		Stencil{

		Ref 2
		// ReadMask 2
		Comp NotEqual
		}

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	sampler2D _MainTex;

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.uv);
	// just invert the colors
	col = fixed4(0.0,0.0,0.8,1.0);
	/*return fixed4(0.0, 1.0, 0.0, 1.0);*/
	return col;
	}
		ENDCG
	}

	// Pass 2
	Pass
	{
		Stencil{

		Ref 2
		// ReadMask 2
		Comp NotEqual
	}

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag

#include "UnityCG.cginc"

		struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float2 uv : TEXCOORD0;
		float4 vertex : SV_POSITION;
	};

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	sampler2D _MainTex;

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.uv);
	// just invert the colors
	col.rgb = col.rgb + fixed3(0.0,1.0,0.0);
	/*return fixed4(0.0, 1.0, 0.0, 1.0);*/
	return col;
	}
		ENDCG
	}

	}
}
