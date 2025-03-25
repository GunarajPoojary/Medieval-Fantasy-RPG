Shader "RealFit/ColorReplace"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TargetColor ("Target Color", Color) = (1, 1, 1, 1)
        _Tolerance ("Tolerance", Float) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
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
            float4 _TargetColor;
            float _Tolerance;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture
                float4 texColor = tex2D(_MainTex, i.uv);

                // Keep transparency intact
                if (texColor.a == 0)
                {
                    return float4(0, 0, 0, 0);
                }

                // Replace black color with target color
                if (texColor.rgb.r < _Tolerance && texColor.rgb.g < _Tolerance && texColor.rgb.b < _Tolerance)
                {
                    return float4(_TargetColor.rgb, texColor.a); // Keep the original alpha
                }

                return texColor;
            }
            ENDCG
        }
    }
}