Shader "Custom/RainWiperEffect"
{
    Properties
    {
        _MainTex("Rain Texture", 2D) = "white" {}
        _WiperMask("Wiper Mask", 2D) = "black" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Overlay" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            sampler2D _WiperMask;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 rainTex = tex2D(_MainTex, i.uv);   // Textura de la lluvia
                fixed4 mask = tex2D(_WiperMask, i.uv);     // Render Texture del limpiaparabrisas

                // Combina la textura de la lluvia y la máscara del limpiaparabrisas
                float alpha = 1.0 - mask.r; // Área limpia en negro (0) y lluvia en blanco (1)
                return rainTex * alpha;
            }
            ENDCG
        }
    }
}
