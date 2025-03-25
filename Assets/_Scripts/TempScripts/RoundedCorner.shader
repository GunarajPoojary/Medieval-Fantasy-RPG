Shader "RealFit/RoundedRect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Radius ("Corner Radius", Float) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Radius;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv * 2.0 - 1.0;
                float4 col = tex2D(_MainTex, i.uv);
                float dist = length(max(abs(uv) - float2(1.0 - _Radius, 1.0 - _Radius), 0.0));
                
                if (dist > _Radius) discard;
                
                return col;
            }
            ENDCG
        }
    }
}