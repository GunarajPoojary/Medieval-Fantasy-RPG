Shader "Custom/AdvancedGradientOverlay"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _GradientColor1 ("Gradient Start Color", Color) = (1,0,0,1)
        _GradientColor2 ("Gradient End Color", Color) = (0,0,1,1)
        _Direction ("Gradient Direction (0=Horizontal, 1=Vertical, 2=Radial)", Float) = 0
        _Center ("Gradient Center (for Radial)", Vector) = (0.5, 0.5, 0, 0)
        _Shape ("Gradient Shape Factor", Float) = 1.0
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            fixed4 _GradientColor1;
            fixed4 _GradientColor2;
            float _Direction;
            float2 _Center;
            float _Shape;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the base texture
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Gradient factor initialization
                float gradientFactor = 0.0;

                // Calculate gradient direction and shape
                if (_Direction == 0) // Horizontal
                {
                    gradientFactor = i.uv.x;
                }
                else if (_Direction == 1) // Vertical
                {
                    gradientFactor = i.uv.y;
                }
                else if (_Direction == 2) // Radial
                {
                    float2 centerUV = _Center.xy;
                    float distance = length(i.uv - centerUV);
                    gradientFactor = pow(distance, _Shape); // Apply shape factor
                }

                // Smooth gradient (optional)
                gradientFactor = saturate(gradientFactor);

                // Interpolate between gradient colors
                fixed4 gradientColor = lerp(_GradientColor1, _GradientColor2, gradientFactor);

                // Combine the texture color with the gradient
                fixed4 finalColor = texColor * (1 - gradientColor.a) + gradientColor * gradientColor.a;
                return finalColor;
            }
            ENDCG
        }
    }
    FallBack "Transparent/VertexLit"
}
