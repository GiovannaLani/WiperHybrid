Shader "Custom/TriangleFadeToBlack"
{
    Properties
    {
        _FadeStart("Fade Start (0-1)", Range(0,1)) = 0.4    // Desde dónde empieza el degradado
        _FadeEnd("Fade End (0-1)", Range(0,5)) = 0.8        // Dónde termina y es negro total
        _Intensity("Black Intensity", Range(0,1)) = 1.0     // Qué tan negro es
    }
        SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float _FadeStart;
            float _FadeEnd;
            float _Intensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Calcula cuánto negro aplicar según la posición horizontal
                float fade = smoothstep(_FadeStart, _FadeEnd, i.uv.x);
                float alpha = fade * _Intensity;

                return fixed4(0.0, 0.0, 0.0, alpha); // Negro con alpha según degradado
            }
            ENDCG
        }
    }
}
