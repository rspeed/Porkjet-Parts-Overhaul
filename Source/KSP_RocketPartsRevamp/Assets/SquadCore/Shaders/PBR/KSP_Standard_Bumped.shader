Shader "KSP/Standard_Bumped"
{
    Properties
    {
        [Header(Texture Maps)]
        _MainTex("Map A (RGB albedo, A smoothness)", 2D) = "white" {}
        _MapB("Map B (R metalness, G emission1, B emission2, A occlusion)", 2D) = "white" {}
        [Space]
        _BumpMap("BumpMap", 2D) = "bump" {}

        [Header(Emissive)]
        _EmissiveColorLow1("_EmissiveColorLow1", Color) = (0,0,0,1)
        _EmissiveColorHigh1("_EmissiveColorHigh1", Color) = (0,0,0,1)
        [Space]
        _EmissiveColorLow2("_EmissiveColorLow2", Color) = (0,0,0,1)
        _EmissiveColorHigh2("_EmissiveColorHigh2", Color) = (0,0,0,1)
       
        [Header(Other)]
        //[Toggle(ENABLE_VC)] _VC("Vertex Colors enable", Int) = 0
        //[Toggle(ENABLE_VCAO)] _VCAO("Vertex Color R = Occlusion", Int) = 0
        [KeywordEnum(none, RGBA, Occlusion)] _VCOL("Vertex Color", Float) = 0

        _Opacity("Opacity", Range(0,1)) = 1

        [HideInInspector]_RimFalloff("Rim highlight falloff", Range(0.01,5)) = 0.1
        [HideInInspector]_RimColor("Rim highlight color", Color) = (0,0,0,0)
        [HideInInspector]_TemperatureColor("Heat color", Color) = (0,0,0,0)
        [HideInInspector]_BurnColor("Burn color", Color) = (1,1,1,1)

    }

        SubShader
    {
        Tags{ "RenderType" = "Opaque" }
        ZWrite On
        ZTest LEqual
        Blend SrcAlpha OneMinusSrcAlpha

        CGPROGRAM

        #pragma surface surf Standard keepalpha
        #pragma target 3.0
        #pragma multi_compile _VCOL_NONE _VCOL_RGBA _VCOL_OCCLUSION


        //#pragma shader_feature ENABLE_VCAO
        //#pragma shader_feature ENABLE_VC


        sampler2D _MainTex;
        sampler2D _MapB;
        sampler2D _BumpMap;

        float4 _EmissiveColorLow1;
        float4 _EmissiveColorHigh1;
        float4 _EmissiveColorLow2;
        float4 _EmissiveColorHigh2;

        float _Opacity;
        float _RimFalloff;
        float4 _RimColor;
        float4 _TemperatureColor;
        float4 _BurnColor;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MapB;

            float2 uv_BumpMap;
            float3 viewDir;
            #if _VCOL_RGBA
            float4 color : COLOR;   //vertex color
            #endif
            #if _VCOL_OCCLUSION
            float4 color : COLOR;   //vertex color
            #endif
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float4 mainSample = tex2D(_MainTex, (IN.uv_MainTex));
            float4 mapBSample = tex2D(_MapB, (IN.uv_MapB));
            float3 albedo = mainSample.rgb * _BurnColor.rgb;
            float3 normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

            half rim = 1.0 - saturate(dot(normalize(IN.viewDir), normal));

            float3 emission = (_RimColor.rgb * pow(rim, _RimFalloff)) * _RimColor.a;
            emission += _TemperatureColor.rgb * _TemperatureColor.a;
            emission += lerp(_EmissiveColorLow1.rgb, _EmissiveColorHigh1.rgb, mapBSample.g)*mapBSample.g + lerp(_EmissiveColorLow2.rgb, _EmissiveColorHigh2.rgb, mapBSample.b)*mapBSample.b;

            o.Albedo = albedo;
            o.Emission = emission.rgb;
            o.Smoothness = mainSample.a* _BurnColor.r;
            o.Metallic = mapBSample.r;
            o.Occlusion = mapBSample.a;
            o.Normal = normal;

            o.Emission *= _Opacity;
            o.Alpha = _Opacity;


            #if _VCOL_RGBA
                o.Albedo *= IN.color.rgb;
                o.Occlusion *= IN.color.a;           
            #endif
            #if _VCOL_OCCLUSION
                o.Smoothness *= IN.color.r;
                o.Occlusion *= IN.color.r;
            #endif

//                o.Albedo *= float3(1, 0, 0);

 //           #if ENABLE_VC
 //               #if !ENABLE_VCAO
 //               o.Albedo *= IN.color.rgb;
  //              o.Occlusion *= IN.color.a;
  //              #endif
//
 //               #if ENABLE_VCAO
 //               o.Smoothness *= IN.color.r;
 //               o.Occlusion *= IN.color.r;
 //               #endif
 //           #endif
        }
    ENDCG
    }
    Fallback "Standard"
}