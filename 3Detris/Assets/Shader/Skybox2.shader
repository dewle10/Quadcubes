Shader "Unlit/Skybox2"
{
    Properties
    {
        _TopColor ("Top Color", Color) = (0.5,0.7,1,1)
        _BottomColor ("Bottom Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "Queue"="Background" "RenderType"="Opaque" }
        Cull Off ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _TopColor;
            fixed4 _BottomColor;

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 dir : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.dir = v.vertex.xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = saturate(i.dir.y * 0.5 + 0.5);
                return lerp(_BottomColor, _TopColor, t);
            }
            ENDCG
        }
    }
    FallBack Off
}

