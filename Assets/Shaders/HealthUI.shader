Shader "Unlit/HealthUI"
{
    Properties
    {   
        _Color("Color", Color) = (1.0, 1.0, 1.0, 1.0)
        _MainTex ("Notch Mask", 2D) = "white" {}
        _Shear ("Shear", float) = 0

        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend One OneMinusSrcAlpha
        ColorMask [_ColorMask]

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
            sampler2D _MainTex;
            float _Shear;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                float segCount = i.color.r * 255.0;
                uv.x *= segCount;

                uv.x += (-uv.y) * _Shear;
                uv.x *= 1 + (_Shear / segCount);

                if (uv.x < 0) discard;
                if (uv.x > segCount) discard;

                float segSpacing = i.color.g;
                float correction = (segCount * (1.0 - segSpacing) + (segCount - 1) * segSpacing) / segCount;
                uv.x *= correction;
                uv.x = (uv.x % 1) / (1 - segSpacing);

                if (uv.x > 1) discard;

                fixed4 col = tex2D(_MainTex, uv);

                col *= _Color;

                return col;
            }
            ENDCG
        }
    }
}
