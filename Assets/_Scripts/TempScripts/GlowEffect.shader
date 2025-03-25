Shader "Custom/GlowEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1, 1, 1, 1)
        _GlowIntensity ("Glow Intensity", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _GlowColor;
            float _GlowIntensity;

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

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float4 mainTex = tex2D(_MainTex, i.uv);
                float alpha = mainTex.a;

                // Calculate the glow effect
                float glow = smoothstep(0.0, 0.5, alpha) * _GlowIntensity;

                // Add the glow to the texture
                float4 color = mainTex;
                color.rgb += glow * _GlowColor.rgb;

                return float4(color.rgb, alpha);
            }
            ENDCG
        }
    }
}
