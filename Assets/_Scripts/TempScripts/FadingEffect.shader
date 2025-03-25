Shader "UI/FadingEffect"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" {}
        _FadeDirection ("Fade Direction", Vector) = (1, 0, 0, 0) // Default: Left-to-Right
        _FadeAmount ("Fade Amount", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "CanvasModulateColor" = "1" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _FadeDirection; // Direction of the fade effect
            float _FadeAmount;     // How far the fade progresses (0 = no fade, 1 = full fade)

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float2 screenPos : TEXCOORD1;
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                // Pass screen-space X and Y positions to the fragment shader
                o.screenPos = v.vertex.xy;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the texture color
                fixed4 col = tex2D(_MainTex, i.uv);

                // Calculate fade based on the direction and screen-space position
                float fade = dot(i.screenPos, _FadeDirection.xy);
                fade = saturate((fade + _FadeAmount) / (1 + _FadeAmount));

                // Apply the fade to the alpha channel
                col.a *= fade;

                return col;
            }
            ENDCG
        }
    }

    FallBack "UI/Default"
}
