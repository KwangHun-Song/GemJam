Shader "Custom/MatCap"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Matcap("Matcap",2D) = "white"{}
        _TintColor("TintColor",Color) = (1,1,1,1)
        _TintRange("TintRange",Range(0,2)) = 1
 //     _BumpMap("NormalMap",2D) = "white"{}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        blend SrcAlpha OneMinusSrcAlpha        

        CGPROGRAM
        #pragma surface surf nolight noambient noshadow

        sampler2D _MainTex;
        sampler2D _Matcap;
        fixed4 _TintColor;
        fixed _TintRange;
//      sampler2D _BumpMap;

        struct Input
        {
            fixed2 uv_MainTex;
            fixed3 worldNormal;
 //         fixed2 uv_BumpMap;
 //         INTERNAL_DATA
        };



        void surf (Input IN, inout SurfaceOutput o)
        {            

            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);

//          o.Normal = UnpackNormal(tex2D(_BumpMap,IN.uv_BumpMap));
//          fixed3 worldNor = WorldNormalVector(IN, o.Normal);

            //맵캡
            fixed3 viewNormal = mul((fixed3x3)UNITY_MATRIX_V, IN.worldNormal.rgb);
            fixed2 MatcapUV = viewNormal.xy * 0.5 + 0.5;
            
            //o.Emission = tex2D(_Matcap, MatcapUV) * c.rgb + (c.rgb * _TintRange) * _TintColor;
            o.Emission = tex2D(_Matcap, MatcapUV) * c.rgb * _TintColor * _TintRange;           
            o.Alpha = c.a;
        }

        fixed4 Lightingnolight(SurfaceOutput s, fixed3 lightDir, fixed viewDir, fixed atten)
        {
            return fixed4(0,0,0,s.Alpha);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
