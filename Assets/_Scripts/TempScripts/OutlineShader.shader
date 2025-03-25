Shader "Custom/OutlineShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness("Outline Thickness", Range(1, 10)) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _OutlineColor;
            float _OutlineThickness;

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

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float2 texelSize = _OutlineThickness / _ScreenParams.xy;

                // Sample neighboring pixels
                float4 col = tex2D(_MainTex, i.uv);
                float4 colUp = tex2D(_MainTex, i.uv + float2(0, texelSize.y));
                float4 colDown = tex2D(_MainTex, i.uv - float2(0, texelSize.y));
                float4 colLeft = tex2D(_MainTex, i.uv - float2(texelSize.x, 0));
                float4 colRight = tex2D(_MainTex, i.uv + float2(texelSize.x, 0));

                // Calculate outline
                float alpha = col.a;
                alpha += colUp.a + colDown.a + colLeft.a + colRight.a;

                if (alpha > 0.5)
                {
                    return float4(_OutlineColor.rgb, 1.0); // Outline color
                }

                return col; // Original image color
            }
            ENDCG
        }
    }
}
