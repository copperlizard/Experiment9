Shader "Custom/InteractiveSurfaceShader" {
	Properties {
        _EdgeLength ("Edge length", Range(2,50)) = 15
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _DispTex ("Disp Texture", 2D) = "gray" {}
        _NormalMap ("Normalmap", 2D) = "bump" {}
        _Displacement ("Displacement", Range(0, 1.0)) = 0.3
        _Color ("Color", color) = (1,1,1,0)
        _SpecColor ("Spec color", color) = (0.5,0.5,0.5,0.5)
		_NormalScanTriSideLength ("NormalScanTriSideLength", float) = 0.01
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 300
		    
        CGPROGRAM
        #pragma surface surf BlinnPhong addshadow fullforwardshadows vertex:vert tessellate:tessEdge nolightmap
        #pragma target 4.6
        #include "Tessellation.cginc"

        struct appdata {
            float4 vertex : POSITION;
            float4 tangent : TANGENT;
            float3 normal : NORMAL;
            float2 texcoord : TEXCOORD0;
        };

		struct Input {
            float2 uv_MainTex;
        };

		sampler2D _MainTex;
        sampler2D _NormalMap;
        fixed4 _Color;

		sampler2D _DispTex;
        float _Displacement;
		float _NormalScanTriSideLength;

		sampler2D _DispHistTex; //displacement history texture

        float _EdgeLength;

        float4 tessEdge (appdata v0, appdata v1, appdata v2)
        {
            return UnityEdgeLengthBasedTess (v0.vertex, v1.vertex, v2.vertex, _EdgeLength);
        }

		float3 FindSurfaceNormal (float2 texCoord)
		{	
			//Draw triangle around point, find triangle normal...
			float3 pos = float3(texCoord.x, 0.0, texCoord.y);
			float halfH = ((_NormalScanTriSideLength * 1.73205)/2.0)/2.0;
			float3 m = pos - float3(0.0, 0.0, halfH);
			float3 p1 = pos + float3(0.0, 0.0, halfH);
			float3 p2 = m + float3(_NormalScanTriSideLength * 0.5, 0.0, 0.0);
			float3 p3 = m - float3(_NormalScanTriSideLength * 0.5, 0.0, 0.0);

			p1.y = -tex2Dlod(_DispTex, float4(-p1.xz, 0.0, 0.0)).r * _Displacement;
			p2.y = -tex2Dlod(_DispTex, float4(-p2.xz, 0.0, 0.0)).r * _Displacement;
			p3.y = -tex2Dlod(_DispTex, float4(-p3.xz, 0.0, 0.0)).r * _Displacement;

			float3 p1p2 = p2 - p1, p1p3 = p3 - p1;

			return normalize(cross(normalize(p1p2), normalize(p1p3)));
			//return pos;
			//return float3(1.0, 1.0, 1.0) * tex2Dlod(_DispTex, float4(-texCoord.xy, 0.0, 0.0)).r;
		}

        void vert (inout appdata v)
        {   
            float d = -tex2Dlod(_DispTex, float4(-v.texcoord.xy, 0.0, 0.0)).r * _Displacement;
			v.vertex.xyz += v.normal * d;
        }

        void surf (Input IN, inout SurfaceOutput o) {
            half4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            o.Specular = 0.2;
            o.Gloss = 1.0;
            o.Normal = UnpackNormal(tex2D(_NormalMap, IN.uv_MainTex));
			o.Normal = lerp(o.Normal, FindSurfaceNormal(IN.uv_MainTex), 0.8);
			//o.Normal = FindSurfaceNormal(IN.uv_MainTex);
			//o.Albedo = o.Normal;
        }
        ENDCG
    }
    FallBack "Diffuse"
}