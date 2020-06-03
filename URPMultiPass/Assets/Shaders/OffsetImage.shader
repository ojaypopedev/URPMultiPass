Shader "Hidden/OffsetImage"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _CameraColorTexture;

            fixed4 frag (v2f i) : SV_Target
            {
                half colR = tex2D(_CameraColorTexture, i.uv + half2(-0.01,0)).r;
                half colG = tex2D(_CameraColorTexture, i.uv + half2(0.01,0)).g;
                half colB = tex2D(_CameraColorTexture, i.uv + half2(0,0.01)).b;

                // just invert the colors
                half4 outColor = half4(colR, colG, colB, 1);
                return outColor;
            }
            ENDCG
        }
    }
}
