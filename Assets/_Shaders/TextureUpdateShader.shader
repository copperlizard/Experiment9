Shader "Unlit/TextureUpdateShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TexHist ("Texture", 2D) = "white" {}
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
				//fixed4 col = tex2D(_MainTex, float2(i.uv.x, 1.0 - i.uv.y));

				float hstep = 2.0, vstep = 2.0, blur = 0.5;

				//float4 col = float4(0.0, 0.0, 0.0, 0.0);
				//col = tex2D(_MainTex, uv);

				fixed4 col = fixed4(0.0, 0.0, 0.0, 0.0);
				col = tex2D(_MainTex, float2(i.uv.x - 4.0 * blur * hstep, 1.0 - i.uv.y - 4.0 * blur * vstep)) * 0.0162162162;
				col += tex2D(_MainTex, float2(i.uv.x - 3.0 * blur * hstep, 1.0 - i.uv.y - 3.0 * blur * vstep)) * 0.0540540541;
				col += tex2D(_MainTex, float2(i.uv.x - 2.0 * blur * hstep, 1.0 - i.uv.y - 2.0 * blur * vstep)) * 0.1216216216;
				col += tex2D(_MainTex, float2(i.uv.x - 1.0 * blur * hstep, 1.0 - i.uv.y - 1.0 * blur * vstep)) * 0.1945945946;
	
				col += tex2D(_MainTex, float2(i.uv.x, 1.0 - i.uv.y)) * 0.2270270270;
	
				col += tex2D(_MainTex, float2(i.uv.x + 1.0 * blur * hstep, 1.0 - i.uv.y + 1.0 * blur * vstep)) * 0.1945945946;
				col += tex2D(_MainTex, float2(i.uv.x + 2.0 * blur * hstep, 1.0 - i.uv.y + 2.0 * blur * vstep)) * 0.1216216216;
				col += tex2D(_MainTex, float2(i.uv.x + 3.0 * blur * hstep, 1.0 - i.uv.y + 3.0 * blur * vstep)) * 0.0540540541;
				col += tex2D(_MainTex, float2(i.uv.x + 4.0 * blur * hstep, 1.0 - i.uv.y + 4.0 * blur * vstep)) * 0.0162162162;



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
