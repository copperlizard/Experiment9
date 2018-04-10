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

			fixed3 DepthtoCol (float t)
			{
				if(t == 0)
				{
					return fixed3(0.0, 0.0, 0.0);
				}

				static float pi = 3.14159265358979323846;
				fixed3 a = fixed3(0.5, 0.5, 0.5), b = fixed3(0.5, 0.5, 0.5), c = fixed3(1.0, 1.0, 1.0), d = fixed3(0.0, 0.33, 0.67);
				return (a + b * cos(2.0 * pi * (c * t + d)));
			}

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
				// sample the texture
				//fixed4 col = fixed4(DtoCol(tex2D(_MainTex, float2(i.uv.x, 1.0 - i.uv.y)).r), 1.0);
				
				float d = tex2D(_MainTex, float2(i.uv.x, 1.0 - i.uv.y)).r;
				float s = 1.0 - tex2D(_HeightMap, i.uv).r * 2.0;

				float dif = s - d;

				float inter = 1.0 - smoothstep(0.0, 0.025, dif);

				// d... 1 == sky, 0 == ground
				// s... 0 == no height, 1 == max height

				//fixed4 col = fixed4(s, 0.0, 0.0, 1.0);
				
				//Height map of player
				//fixed4 col = fixed4(DepthtoCol(d) * inter, 1.0);

				//Height map of surface
				//fixed4 col = fixed4(DepthtoCol(s), 1.0);		
				
				fixed4 col = fixed4(inter, 0.0, 0.0, 0.0);


				//NEED TO RUN TERRAIN DATA THROUGH DEPTH TO COL AND COMPARE COLORS, USE DIF TO DETERMINE WHAT BLENDS INTO TRACKS AND WHAT DOESNT!!!

				
				/*float hstep = 2.0, vstep = 2.0, blur = 0.5;

				fixed4 col = fixed4(0.0, 0.0, 0.0, 0.0);
				col = tex2D(_MainTex, float2(i.uv.x - 4.0 * blur * hstep, 1.0 - i.uv.y - 4.0 * blur * vstep)) * 0.0162162162;
				col += tex2D(_MainTex, float2(i.uv.x - 3.0 * blur * hstep, 1.0 - i.uv.y - 3.0 * blur * vstep)) * 0.0540540541;
				col += tex2D(_MainTex, float2(i.uv.x - 2.0 * blur * hstep, 1.0 - i.uv.y - 2.0 * blur * vstep)) * 0.1216216216;
				col += tex2D(_MainTex, float2(i.uv.x - 1.0 * blur * hstep, 1.0 - i.uv.y - 1.0 * blur * vstep)) * 0.1945945946;
	
				col += tex2D(_MainTex, float2(i.uv.x, 1.0 - i.uv.y)) * 0.2270270270;
	
				col += tex2D(_MainTex, float2(i.uv.x + 1.0 * blur * hstep, 1.0 - i.uv.y + 1.0 * blur * vstep)) * 0.1945945946;
				col += tex2D(_MainTex, float2(i.uv.x + 2.0 * blur * hstep, 1.0 - i.uv.y + 2.0 * blur * vstep)) * 0.1216216216;
				col += tex2D(_MainTex, float2(i.uv.x + 3.0 * blur * hstep, 1.0 - i.uv.y + 3.0 * blur * vstep)) * 0.0540540541;
				col += tex2D(_MainTex, float2(i.uv.x + 4.0 * blur * hstep, 1.0 - i.uv.y + 4.0 * blur * vstep)) * 0.0162162162;*/



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
