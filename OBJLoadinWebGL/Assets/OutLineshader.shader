Shader "Custom/OutLineshader" {
	Properties {
        _MainTex ("Albedo", 2D) = "white" {}

        _OutlineColor("OutlineColor", Color) = (0,1,0,1)
        _Outline("Outline", Range(0.01, 0.04)) = 0.04
    }
 
  SubShader {

        Tags { "RenderType"="Opaque" }
        Cull front
 
        // Pass1
        CGPROGRAM
        #pragma surface surf NoLighting vertex:vert noshadow noambient
 
        sampler2D _MainTex;

        struct Input {
            float2 uv_MainTex;

        };
 
        fixed4 _OutlineColor;
        float _Outline;
 
        void vert(inout appdata_full v)
        {
            v.vertex.xyz += v.normal.xyz * _Outline;
        }
 
        void surf(Input In, inout SurfaceOutput o)
        {
        }
        
        fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            return _OutlineColor;
        }
        ENDCG
 
        // Pass2
        Cull back
        CGPROGRAM
        #pragma surface surf Toon
        
        fixed4 _Color;
        sampler2D _MainTex;
        sampler2D _BumpMap;
        struct Input {
            float2 uv_MainTex;
            fixed4 color : Color;
        };
 
        void surf(Input In, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, In.uv_MainTex);
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
 
        fixed4 LightingToon(SurfaceOutput s, fixed3 lightDir, fixed atten)
        {
            fixed halfLambert = dot(s.Normal, lightDir) * 0.5 + 0.5;
            halfLambert = ceil(halfLambert * 5) / 5;
            
            fixed4 final;
            final.rgb = s.Albedo * halfLambert *_LightColor0.rgb;
            final.a = s.Alpha;
            return final;
        }
        ENDCG
    }
    FallBack "Diffuse"


}
