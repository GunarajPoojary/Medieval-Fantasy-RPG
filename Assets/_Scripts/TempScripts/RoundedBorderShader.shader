Shader "Custom/RoundedBorderShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BorderColor ("Border Color", Color) = (1, 0, 0, 1)
        _BorderThickness ("Border Thickness", Range(0.0, 0.1)) = 0.05
        _CornerRadius ("Corner Radius", Range(0.0, 0.5)) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }
        LOD 100

        Pass
        {
            Name "RoundedBorder"
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _BorderColor;
            float _BorderThickness;
            float _CornerRadius;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float RectBorder(float2 uv, float radius, float thickness)
            {
                // Center UV to 0.5, 0.5
                float2 centered = abs(uv - 0.5);
                
                // Distance from center to edge of rounded rectangle
                float edgeDist = max(centered.x - 0.5 + radius, centered.y - 0.5 + radius);
                
                // Smooth border calculation
                return smoothstep(radius + thickness, radius, edgeDist);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Texture sampling
                fixed4 texColor = tex2D(_MainTex, i.uv);
                
                // Calculate border mask
                float borderMask = RectBorder(i.uv, _CornerRadius, _BorderThickness);
                
                // Blend border color with texture
                fixed4 borderColor = _BorderColor * (1.0 - borderMask);
                
                return texColor * borderMask + borderColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}