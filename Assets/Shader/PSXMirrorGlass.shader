Shader "PSX/MirrorGlass"
{
    Properties
    {
        _Tint ("Tom do vidro", Color) = (0.55, 0.72, 0.78, 1)
        _BaseAlpha ("Opacidade base", Range(0,1)) = 0.12
        _FresnelAlpha ("Opacidade nas bordas (angulo)", Range(0,1)) = 0.45
        _FresnelPower ("Forca do fresnel", Range(0.5, 6)) = 2.5

        _StreakColor ("Cor do brilho diagonal", Color) = (1,1,1,1)
        _StreakAlpha ("Opacidade do brilho", Range(0,1)) = 0.28
        _StreakWidth ("Largura do brilho", Range(0.01, 0.5)) = 0.07
        _StreakSpeed ("Velocidade do brilho (0 = parado)", Float) = 0.05
        _StreakSteps ("Passos do brilho (visual PSX)", Float) = 12

        _Dirt ("Textura de sujeira (Point filter)", 2D) = "white" {}
        _DirtAmount ("Quantidade de sujeira", Range(0,1)) = 0.35

        _PixelGrid ("Grade de pixels (UV)", Float) = 64
        _EdgeDark ("Escurecimento das bordas", Range(0,1)) = 0.6
        _EdgeSize ("Tamanho da borda escura", Range(0.01, 0.4)) = 0.12
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Tint, _StreakColor;
            float _BaseAlpha, _FresnelAlpha, _FresnelPower;
            float _StreakAlpha, _StreakWidth, _StreakSpeed, _StreakSteps;
            sampler2D _Dirt;
            float4 _Dirt_ST;
            float _DirtAmount, _PixelGrid, _EdgeDark, _EdgeSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = _WorldSpaceCameraPos - worldPos;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Trava o UV numa grade, deixa tudo "pixelado" estilo PSX
                float2 uv = floor(i.uv * _PixelGrid) / _PixelGrid;

                // Fresnel: o vidro fica mais "espelhado" quando visto de lado
                float3 n = normalize(i.worldNormal);
                float3 v = normalize(i.viewDir);
                float fres = pow(1.0 - saturate(abs(dot(n, v))), _FresnelPower);

                // Brilho diagonal (o "chuvisco" de luz tipico de espelho)
                float t = floor(_Time.y * _StreakSteps) / _StreakSteps; // tempo em passos, cara de PSX
                float d = uv.x + uv.y * 0.65 - t * _StreakSpeed;
                float band1 = step(abs(frac(d) - 0.35), _StreakWidth);
                float band2 = step(abs(frac(d) - 0.55), _StreakWidth * 0.4);
                float streak = saturate(band1 + band2);

                // Sujeira / digitais / poeira
                float2 dirtUV = uv * _Dirt_ST.xy + _Dirt_ST.zw;
                float dirt = (1.0 - tex2D(_Dirt, dirtUV).r) * _DirtAmount;

                // Borda escura (cantos do espelho meio oxidados)
                float2 e = min(uv, 1.0 - uv);
                float edge = 1.0 - smoothstep(0.0, _EdgeSize, min(e.x, e.y));
                edge = floor(edge * 4.0) / 4.0; // posterizado

                // Composicao
                float3 col = _Tint.rgb;
                col = lerp(col, _StreakColor.rgb, streak);
                col = lerp(col, float3(0.15, 0.15, 0.12), dirt);
                col *= 1.0 - edge * _EdgeDark;

                float alpha = _BaseAlpha
                            + fres * _FresnelAlpha
                            + streak * _StreakAlpha
                            + dirt * 0.5
                            + edge * _EdgeDark * 0.5;

                return fixed4(col, saturate(alpha));
            }
            ENDCG
        }
    }
}
