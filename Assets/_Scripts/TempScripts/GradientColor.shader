Shader "RealFit/GradientColor"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (1, 1, 1, 1)
        _BottomColor ("Bottom Color", Color) = (0, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _TopColor;
            fixed4 _BottomColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = v.vertex;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float gradient = i.worldPos.y;
                return lerp(_BottomColor, _TopColor, gradient);
            }
            ENDCG
        }
    }
}
