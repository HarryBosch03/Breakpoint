Shader "Unlit/BufferUI"
{
    Properties
    {
        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float4 _Color;

            fixed4 frag (v2f i) : SV_Target
            {
                float segCount = i.color.r * 255.0;
                float segSpacing = i.color.g;

                float correction = (segCount * (1.0 - segSpacing) + (segCount - 1) * segSpacing) / segCount;
                if((i.uv.x * correction * segCount) % 1.0 > 1 - segSpacing) discard;

                return _Color;
            }
            ENDCG
        }
    }
}
