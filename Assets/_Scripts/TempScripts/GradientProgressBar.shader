Shader "Custom/GradientProgressBar"
{
    Properties
    {
        _Color1 ("Color 1", Color) = (1, 0, 0, 1)
        _Color2 ("Color 2", Color) = (0, 1, 0, 1)
        _Progress ("Progress", Range(0, 1)) = 0.5
        _MainTex ("Base (RGB)", 2D) = "white" { }
    }
    SubShader
    {
        Tags { "RenderType"="UI" }
        
        Pass
        {
            ZWrite On
            Cull Front
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            uniform float _Progress; // The progress value
            uniform float4 _Color1;  // First color of the gradient
            uniform float4 _Color2;  // Second color of the gradient
            sampler2D _MainTex; // Texture for the image

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Get the progress value based on the x-coordinate of the UV
                float progress = _Progress; // Retrieve the shader progress value
                float3 color = lerp(_Color1.rgb, _Color2.rgb, progress); // Interpolate colors
                return half4(color, 1.0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
