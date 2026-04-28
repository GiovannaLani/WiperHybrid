Shader "Custom/BlackCoverFromTop"
{
    Properties
    {
        _BaseColor("Base Color", Color) = (1, 1, 1, 1)  // Color blanco
        _CoverColor("Cover Color", Color) = (0, 0, 0, 1)  // Color negro
        _CoverAmount("Cover Amount", Range(0, 5)) = 0.0  // Variable de transición
        _MainTex("Base Texture", 2D) = "white" { }
    }
        SubShader
    {
        Tags { "Queue" = "AlphaTest" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;  // Guardamos la posición mundial para calcular la cobertura
            };

            float _CoverAmount;
            float4 _BaseColor;
            float4 _CoverColor;
            sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;  // Calculamos la posición mundial
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Cálculo para cubrir gradualmente desde arriba hacia abajo
                float coverFactor = smoothstep(0.0, 1.0, i.worldPos.y / _CoverAmount);  // Controlamos cuánto se cubre el objeto basado en la posición Y

                // Mezclamos el color base y el color de la cobertura (negro)
                half4 baseColor = tex2D(_MainTex, i.pos.xy);  // Color base (por defecto blanco)
                half4 finalColor = lerp(baseColor, _CoverColor, coverFactor);  // Mezcla según el coverFactor

                return finalColor;
            }
            ENDCG
        }
    }
}
