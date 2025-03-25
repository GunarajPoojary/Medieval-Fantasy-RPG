Shader "UI/ShapeImage"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _Shape ("Shape Type (0=Circle, 1=RoundedRect)", Range(0, 1)) = 0
        _Radius ("Corner Radius", Range(0, 0.5)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
        LOD 100
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            float _Shape;
            float _Radius;

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
                // Get the aspect ratio of the Image (assumes uniform scaling)
                float aspect = _ScreenParams.x / _ScreenParams.y;

                // Normalize UV to [-1, 1] while compensating for aspect ratio
                float2 uv = (i.uv * 2.0 - 1.0);
                uv.x /= aspect;

                float dist = length(uv);     // Distance from center
                float alpha = 1.0;

                if (_Shape < 0.5) // Circle
                {
                    alpha = step(dist, 1.0); // Circle mask
                }
                else // Rounded Rectangle
                {
                    float2 absUV = abs(uv) - float2(1.0 - _Radius, 1.0 - _Radius); 
                    dist = length(max(absUV, 0.0));
                    alpha = step(dist, _Radius);
                }

                return fixed4(_Color.rgb, _Color.a * alpha);
            }
            ENDCG
        }
    }
}
