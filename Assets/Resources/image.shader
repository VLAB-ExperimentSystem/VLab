﻿Shader "Unlit/image"
{
	Properties
	{
		img("Image", 2D) = "white" {}
		sigma("Sigma", Float) = 0.15
		sizex("SizeX",Float) = 2
		sizey("SizeY", Float) = 2
		masktype("MaskType",Int) = 0
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 300

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D img;
			float sigma;
	        float sizex;
			float sizey;
	        int masktype;
			static const float pi = 3.141592653589793238462;
			static const float pi2 = 6.283185307179586476924;

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
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(img,i.uv);
			    i.uv = i.uv - 0.5;
			    if(masktype==0)
				{ }
				else if (masktype == 1)
				{
					if (sqrt(pow(i.uv.x, 2) + pow(i.uv.y, 2))>0.5)
					{
						c.a = 0;
					}
				}
				else if (masktype == 2)
				{
					float d = pow(i.uv.x, 2) + pow(i.uv.y, 2);
					c.a= c.a*exp(-d / (2 * pow(sigma, 2)));
				}

				return c;
			}

			ENDCG
		}
	}
}