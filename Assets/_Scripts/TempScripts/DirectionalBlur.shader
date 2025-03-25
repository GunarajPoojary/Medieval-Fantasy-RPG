Shader "Custom/DirectionalBlur"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _BlurDirection ("Blur Direction", Vector) = (1, 0, 0, 0) // Blur direction (x, y)
        _BlurStrength ("Blur Strength", Float) = 1.0 // Controls the blur amount
        _Samples ("Blur Samples", Int) = 8 // Number of samples for the blur
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _BlurDirection;
            float _BlurStrength;
            int _Samples;

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

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float2 blurDir = normalize(_BlurDirection.xy) * _BlurStrength / _Samples;

                fixed4 color = tex2D(_MainTex, uv); // Base color
                for (int j = 1; j <= _Samples; j++)
                {
                    color += tex2D(_MainTex, uv + j * blurDir);
                    color += tex2D(_MainTex, uv - j * blurDir);
                }

                color /= (2 * _Samples + 1); // Normalize
                return color;
            }
            ENDCG
        }
    }
}
