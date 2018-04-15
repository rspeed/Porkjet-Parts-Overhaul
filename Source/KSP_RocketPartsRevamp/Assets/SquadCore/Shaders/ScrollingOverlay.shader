Shader "KSP/FX/ScrollingOverlay"
{
	Properties 
	{
        [NoScaleOffset] _MainTex("MainTex (RGB Alpha(A))", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
        _TileX("Tile X", Int) = 1
        _TileY("Tile Y", Int) = 1
		_SpeedX("Scroll Speed X", Int) = 0
        _SpeedY("Scroll Speed Y", Int) = 0
	}
	
	SubShader 
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			ZWrite On
			ColorMask 0
		}

		ZWrite On
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha 

		CGPROGRAM

        #include "LightingKSP.cginc"
		#pragma surface surf Unlit noforwardadd noshadow noambient novertexlights alpha:fade
		#pragma target 3.0

		sampler2D _MainTex;

        float _SpeedX;
        float _SpeedY;
        float _TileX;
        float _TileY;



        struct Input
		{
            float2 uv_MainTex;
            float4 screenPos;
        };

		void surf (Input IN, inout SurfaceOutput o)
		{
            
            float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
            screenUV.x += _Time * _SpeedX;
            screenUV.y += _Time * _SpeedY;
            screenUV.x *= _TileX;
            screenUV.y *= _TileY;

            float4 c = tex2D(_MainTex, (screenUV));
			float3 normal = float3(0,0,1);

			o.Albedo = c.rgb * _Color.rgb;
			o.Normal = normal;
			o.Alpha = c.a * _Color.a;
		}
		ENDCG
	}
}