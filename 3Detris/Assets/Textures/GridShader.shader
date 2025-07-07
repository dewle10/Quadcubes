Shader "GridShader"
{
    Properties
    {
        _LineColor       ("Line Color",       Color) = (0,0,0,1)
        _GridSizeX       ("Grid Size X",     Float) = 1.0
        _GridSizeY       ("Grid Size Y",     Float) = 1.0
        _LineWidth       ("Line Width",      Float) = 0.05
        _OffsetX         ("Offset X",        Float) = 0.0
        _OffsetY         ("Offset Y",        Float) = 0.0
        _ClipThreshold   ("Alpha Clip Threshold", Float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="TransparentCutout" "Queue"="Transparent" }
        Cull Back
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv   : TEXCOORD0;
                float4 pos  : SV_POSITION;
            };

            fixed4 _LineColor;
            float  _GridSizeX;
            float  _GridSizeY;
            float  _LineWidth;
            float  _OffsetX;
            float  _OffsetY;
            float  _ClipThreshold;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv * float2(_GridSizeX, _GridSizeY) + float2(_OffsetX, _OffsetY);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 gridDist = abs(frac(i.uv) - 0.5);
                // antialias width based on screen derivative
                float2 fw = fwidth(i.uv);
                float halfW = _LineWidth * 0.5;
                // compute line mask
                float mx = smoothstep(halfW + fw.x, halfW - fw.x, gridDist.x);
                float my = smoothstep(halfW + fw.y, halfW - fw.y, gridDist.y);
                float mask = max(mx, my);

                fixed4 col = _LineColor;
                col.a *= mask;
                // clip to remove transparent areas and avoid white fringes
                clip(col.a - _ClipThreshold);
                return col;
            }
            ENDCG
        }
    }
}
