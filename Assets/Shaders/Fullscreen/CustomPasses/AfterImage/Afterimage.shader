Shader "Custom/AfterImage"
{
    Properties
    {
        _HistoryTex ("History Texture", 2D) = "black" {}
        _BlendAmount ("Blend Amount", Range(0, 0.99)) = 0.9
        [Toggle(_USEFBF_ON)] _UseFBF ("Use Framebuffer Fetch", Float) = 0
        [Toggle(_USEGRAYSCALE_ON)] _UseGrayscale ("Use Grayscale", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "AfterImagePass"

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_local _ _USEFBF_ON
            #pragma multi_compile_local _ _USEGRAYSCALE_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #if defined(_USEFBF_ON)
                FRAMEBUFFER_INPUT_X_HALF(0);
            #endif

            TEXTURE2D(_HistoryTex);
            SAMPLER(sampler_HistoryTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _HistoryTex_ST;
                float _BlendAmount;
            CBUFFER_END

            half3 ToGrayscale(half3 color)
            {
                half gray = dot(color, half3(0.2126, 0.7152, 0.0722));
                return half3(gray, gray, gray);
            }

            half4 Frag(Varyings input) : SV_Target0
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;

                // Leggi frame corrente: FBF o texture normale
                half3 current;
                #if defined(_USEFBF_ON)
                    current = LOAD_FRAMEBUFFER_X_INPUT(0, input.positionCS.xy).rgb;
                #else
                    current = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv).rgb;
                #endif

                // Leggi history
                half3 hist = SAMPLE_TEXTURE2D(_HistoryTex, sampler_HistoryTex, uv).rgb;

                // Grayscale opzionale sulla history
                #if defined(_USEGRAYSCALE_ON)
                    hist = ToGrayscale(hist);
                #endif

                // Blend
                float blend = clamp(_BlendAmount, 0.0, 0.99);
                half3 result = lerp(current, hist, blend);

                return half4(result, 1.0);
            }

            ENDHLSL
        }
    }
}
