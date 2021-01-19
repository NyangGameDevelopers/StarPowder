Shader "Custom/Checkerboard"
{
    Properties
    {
        [HideInInspector]
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

        _ColorA ("Check Color A", Color) = (1,1,1,1)
        _ColorB ("Check Color B", Color) = (0,0,0,0)
        _Resolution("Resolution", Range(1, 40)) = 25
        _Emission("Emission", Range(0, 1)) = 0
        //_Alpha("Alpha", Range(0, 1)) = 0
        _CamFogDist("Camera Fog Distance", Range(0, 100)) = 20
    }
    SubShader
    {
        //Cull off
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        ZWrite off

        //Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        float4 _ColorA;
        float4 _ColorB;
        float _Resolution;
        float _Emission;
        //float _Alpha;

        float _CamFogDist;

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.uv_MainTex.xy;
            float2 uv2 = frac(uv * _Resolution);
            float2 uv3 = step(uv2, 0.5);
            float check = abs(uv3.x - uv3.y);

            float3 col = lerp(_ColorA.rgb, _ColorB.rgb, check);

            // 카메라 거리 기반 포그 적용
            float camDist = distance(IN.worldPos, _WorldSpaceCameraPos);
            float cdRatio = saturate(camDist / _CamFogDist);

            float4 midColor = lerp(_ColorA, _ColorB, 0.5);
            col = lerp(col, midColor.rgb, cdRatio);

            o.Albedo = col;
            o.Emission = col * _Emission;

            float alphaLerp = lerp(1, 0, cdRatio);
            o.Alpha = step(0.5, alphaLerp) + lerp(0.1, 0, cdRatio);
            //o.Alpha = saturate(step(0.5, alphaLerp) + alphaLerp) * _Alpha;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
