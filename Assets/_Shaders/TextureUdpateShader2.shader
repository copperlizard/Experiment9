Shader "Unlit/TextureUdpateShader2"
{
	Properties
	{
		_MainTex ("DepthTexture", 2D) = "white" {}
		_TexHist ("TracksTexture", 2D) = "white" {}
		_HeightMap ("HeightMap", 2D) = "black" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			/*fixed3 DepthtoCol (float t)
			{
				if(t == 0)
				{
					return fixed3(0.0, 0.0, 0.0);
				}

				static float pi = 3.14159265358979323846;
				fixed3 a = fixed3(0.5, 0.5, 0.5), b = fixed3(0.5, 0.5, 0.5), c = fixed3(1.0, 1.0, 1.0), d = fixed3(0.0, 0.33, 0.67);
				return (a + b * cos(2.0 * pi * (c * t + d)));
			}*/

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _TexHist;
			sampler2D _HeightMap;
			float4 _MainTex_ST;
			
			float GetInter(float2 uv)
			{
				//float d = tex2D(_MainTex, float2(uv.x, 1.0 - uv.y)).r;

				float d = tex2D(_MainTex, float2(uv.x, 1.0 - uv.y)).r;
				d += tex2D(_MainTex, float2(uv.x, 1.0 - uv.y) + float2(uv.x, uv.y + 1.3846153846)).r;
				d += tex2D(_MainTex, float2(uv.x, 1.0 - uv.y) + float2(uv.x, uv.y - 3.2307692308)).r;

				float s = 1.0 - tex2D(_HeightMap, uv).a;

				float dif = s - d;

				return (1.0 - smoothstep(0.0, 0.0175, dif));
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{	
				/*float blurInter = 0.0f;
				blurInter = GetInter(float2(i.uv.x, i.uv.y)) * 0.2270270270;
				blurInter += GetInter(float2(i.uv.x, i.uv.y + 1.3846153846)) * 0.3162162162;
				blurInter += GetInter(float2(i.uv.x, i.uv.y - 3.2307692308)) * 0.0702702703;

				fixed4 col = fixed4(blurInter, 0.0, 0.0, 0.0);*/

				float spreadInter = 0.0f;
				spreadInter = GetInter(i.uv + float2(-1.0 / 2048, -1.0 / 2048));
				spreadInter = max(spreadInter, GetInter(i.uv + float2(-1.0 / 2048, 0.0)));
				spreadInter = max(spreadInter, GetInter(i.uv + float2(-1.0 / 2048, 1.0 / 2048)));
				spreadInter = max(spreadInter, GetInter(i.uv + float2(0.0, -1.0 / 2048)));
				spreadInter = max(spreadInter, GetInter(i.uv + float2(0.0, 0.0)));
				spreadInter = max(spreadInter, GetInter(i.uv + float2(0.0, 1.0 / 2048)));
				spreadInter = max(spreadInter, GetInter(i.uv + float2(1.0 / 2048, -1.0 / 2048)));
				spreadInter = max(spreadInter, GetInter(i.uv + float2(1.0 / 2048, 0.0)));
				spreadInter = max(spreadInter, GetInter(i.uv + float2(1.0 / 2048, 1.0 / 2048)));

				fixed4 col = fixed4(spreadInter, 0.0, 0.0, 0.0);

				//fixed4 col = fixed4(GetInter(i.uv), 0.0, 0.0, 0.0);

				// add history (could lerp with bias towards new frame to fade out steps overtime...)
				col += tex2D(_TexHist, i.uv);
				col = clamp(col, 0.0, 1.0);
				
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
