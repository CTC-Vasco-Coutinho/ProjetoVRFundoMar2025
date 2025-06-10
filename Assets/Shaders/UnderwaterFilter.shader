Shader "Custom/UnderwaterFilter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Intensity", Range(0, 1)) = 0.5
        _Distortion ("Distortion", Range(0, 2)) = 0.1
        _UnderwaterColor ("Underwater Color", Color) = (0.1, 0.4, 0.6, 1)
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100
        
        Pass
        {
            Name "UnderwaterFilter"
            Tags { "LightMode"="UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                half _Intensity;
                half _Distortion;
                half4 _UnderwaterColor;
            CBUFFER_END
            
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                output.positionHCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                
                return output;
            }
            
            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Distorção ondulante usando _Time do URP
                float2 distortedUV = input.uv;
                distortedUV.x += sin(input.uv.y * 10.0 + _Time.y) * _Distortion * 0.01;
                distortedUV.y += cos(input.uv.x * 8.0 + _Time.y * 0.8) * _Distortion * 0.01;
                
                // Amostra a textura com distorção
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, distortedUV);
                
                // Aplica cor subaquática
                col = lerp(col, col * _UnderwaterColor, _Intensity);
                
                // Adiciona efeito de vinheta
                float2 center = input.uv - 0.5;
                float vignette = 1.0 - dot(center, center) * 0.8;
                col.rgb *= vignette;
                
                return col;
            }
            ENDHLSL
        }
    }
    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}