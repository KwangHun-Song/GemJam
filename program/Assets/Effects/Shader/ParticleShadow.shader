Shader "Custom/ParticleShadow"
{
    Properties
    {        
        _MainTex ("MainTex", 2D) = "white" {}
        _ShadowDistance("ShadowDistance",Range(-2,2)) = 0
        _ShadowAdjust("ShadowAdjust",Range(0,1)) = 0

        [Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("SrcBlend Mode", Float) = 5
        [Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("DstBlend Mode", Float) = 10
    }
    SubShader
    {
     
        Tags { "RenderType"="Tranparent" "Queue" = "Transparent" "IgnoreProjector" = "True"}
        zwrite Off
        Blend [_SrcBlend] [_DstBlend]

        //1stPass-------------------------------------------------------------      
        cull back

        CGPROGRAM        
        #pragma surface surf nolight keepalpha noforwardadd nolightmap noambient novertexlights noshadow vertex:vert

        fixed _ShadowDistance;

        void vert(inout appdata_full v)
        {            
            v.vertex.xy = fixed2(v.vertex.x, v.vertex.y + _ShadowDistance );
        }

        sampler2D _MainTex;
        fixed _ShadowAdjust;

        struct Input
        {
            float2 uv_MainTex;
            fixed4 color:COLOR;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
           fixed4 c = tex2D (_MainTex, IN.uv_MainTex) ;
            c = c * IN.color;
            o.Emission = 0;           
            o.Alpha = c.a *_ShadowAdjust;
        }
        

       float4 Lightingnolight(SurfaceOutput s, float3 lightDir, float atten)
        {
            return float4(0,0,0,s.Alpha);
        }
        ENDCG
        

        //2ndPass-------------------------------------------------------------
        cull Back
        
         CGPROGRAM        
        #pragma surface surf nolight keepalpha noforwardadd nolightmap noambient novertexlights noshadow 

        sampler2D _MainTex;       

        struct Input
        {
            float2 uv_MainTex;
            float4 color:COLOR;
        };              

        void surf (Input IN, inout SurfaceOutput o)
        {           
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) ;            
            o.Emission = c.rgb * IN.color.rgb;           
            o.Alpha = c.a * IN.color.a;
        }

       float4 Lightingnolight(SurfaceOutput s, float3 lightDir, float atten)
        {
            return float4(0,0,0,s.Alpha);
        }
        ENDCG
    }
   FallBack "Legacy Shaders/Transparent/VertexLit"
}
