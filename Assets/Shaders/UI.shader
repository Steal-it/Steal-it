Shader "UI/UI"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Gradient)]
        [Toggle(USE_GRADIENT)] _UseGradient ("Use Gradient", Float) = 0
        _GradientAngle ("Direction Angle", Range(0, 360)) = 90

        [Space(6)]
        _GradientColor0 ("Stop 1  Color", Color) = (1,0,0,1)
        _GradientStop0  ("Stop 1  Position", Range(0,1)) = 0.0

        _GradientColor1 ("Stop 2  Color", Color) = (0,0,1,1)
        _GradientStop1  ("Stop 2  Position", Range(0,1)) = 1.0

        _GradientColor2 ("Stop 3  Color", Color) = (0,1,0,1)
        _GradientStop2  ("Stop 3  Position", Range(0,1)) = 1.0

        _GradientColor3 ("Stop 4  Color", Color) = (1,1,0,1)
        _GradientStop3  ("Stop 4  Position", Range(0,1)) = 1.0
    }

    SubShader
    {
        Tags
        {
            "Queue"           = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType"      = "Transparent"
            "PreviewType"     = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull     Off
        Lighting Off
        ZWrite   Off
        ZTest    Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "Default"
        CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma shader_feature_local _ USE_GRADIENT

            // ─── Strutture ────────────────────────────────────────────
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex        : SV_POSITION;
                fixed4 color         : COLOR;      
                fixed4 vertexColor   : COLOR1;     
                float2 texcoord      : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;
                float2 rawUV         : TEXCOORD2;  
                UNITY_VERTEX_OUTPUT_STEREO
            };

            // ─── Uniforms base ────────────────────────────────────────
            sampler2D _MainTex;
            fixed4    _Color;
            fixed4    _TextureSampleAdd;
            float4    _ClipRect;
            float4    _MainTex_ST;

            // ─── Uniforms gradient ────────────────────────────────────
            #ifdef USE_GRADIENT
            float  _GradientAngle;
            fixed4 _GradientColor0, _GradientColor1,
                   _GradientColor2, _GradientColor3;
            float  _GradientStop0,  _GradientStop1,
                   _GradientStop2,  _GradientStop3;

            fixed4 SampleGradient(float t)
            {
                t = saturate(t);

                float f01 = saturate((t - _GradientStop0)
                          / max(_GradientStop1 - _GradientStop0, 1e-4));
                float f12 = saturate((t - _GradientStop1)
                          / max(_GradientStop2 - _GradientStop1, 1e-4));
                float f23 = saturate((t - _GradientStop2)
                          / max(_GradientStop3 - _GradientStop2, 1e-4));

                fixed4 c = lerp(_GradientColor0, _GradientColor1, f01);
                c = lerp(c, lerp(_GradientColor1, _GradientColor2, f12),
                         step(_GradientStop1, t));
                c = lerp(c, lerp(_GradientColor2, _GradientColor3, f23),
                         step(_GradientStop2, t));
                return c;
            }
            #endif

            // ─── Vertex ───────────────────────────────────────────────
            v2f vert(appdata_t v)
            {
                v2f OUT;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.worldPosition = v.vertex;
                OUT.vertex        = UnityObjectToClipPos(OUT.worldPosition);
                OUT.texcoord      = TRANSFORM_TEX(v.texcoord, _MainTex);
                OUT.rawUV         = v.texcoord;          // UV grezze [0,1]
                OUT.vertexColor   = v.color;             // colore raw per gradient
                OUT.color         = v.color * _Color;    // colore tintato per path standard
                return OUT;
            }

            // ─── Fragment ─────────────────────────────────────────────
            fixed4 frag(v2f IN) : SV_Target
            {
                half4 texSample = tex2D(_MainTex, IN.texcoord) + _TextureSampleAdd;
                half4 color;

                #ifdef USE_GRADIENT
                    float rad = _GradientAngle * (UNITY_PI / 180.0);
                    float2 dir = float2(cos(rad), sin(rad));
                    float t = dot(IN.rawUV - 0.5, dir) + 0.5;

                    fixed4 gradColor = SampleGradient(t);
                    color = texSample * IN.vertexColor * gradColor;
                #else
                    color = texSample * IN.color;
                #endif

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif

                return color;
            }
        ENDCG
        }
    }
}