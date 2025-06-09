Shader "Custom/URP_WaterSimple"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (0.02, 0.12, 0.2, 0.8)
        _MainTex("Main Texture", 2D) = "white" {}
        _TextureStrength("Texture Strength", Range(0, 1)) = 0.5
        _Speed("Wave Speed", Range(0, 2)) = 0.5
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        LOD 200

        Pass
        {
            Name "Water"
            Tags { "LightMode" = "UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float2 uv : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _TextureStrength;
            float4 _BaseColor;
            float _Speed;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(worldPos);
                OUT.worldPos = worldPos;
                OUT.worldNormal = TransformObjectToWorldNormal(IN.normalOS);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float2 uvScroll = IN.uv + _Time.y * _Speed;
                float4 texColor = tex2D(_MainTex, uvScroll);
                
                // Misturar textura com cor base
                float3 finalColor = lerp(_BaseColor.rgb, texColor.rgb, _TextureStrength);
                
                return float4(finalColor, _BaseColor.a);
            }

            ENDHLSL
        }
    }
}