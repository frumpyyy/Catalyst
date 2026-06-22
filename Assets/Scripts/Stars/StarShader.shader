Shader "Custom/StarShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            Blend SrcAlpha One
            Cull Off
            ZWrite Off

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"


            struct GPU_Star
            {
               float2 position;
               float brightness;
               float speed;
               float phase;
            };

            StructuredBuffer<GPU_Star> stars;

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float brightness : TEXCOORD1;
            };

            Varyings vert(uint id : SV_VertexID)
            {
                Varyings OUT;

                uint starID = id / 6;
                uint squareCorner = id % 6;

                GPU_Star s = stars[starID];

                static const float2 quad[6] = {
                    float2(-1,-1),    
                    float2(-1, 1),    
                    float2( 1, 1),
                    float2(-1,-1),    
                    float2( 1, 1),    
                    float2( 1,-1)    
                };

                float2 offset = quad[squareCorner];

                float size = 0.05f;

                float3 worldPosition = float3(s.position, 0) + float3(offset * size, 0);

                OUT.positionHCS = TransformWorldToHClip(worldPosition);
                OUT.brightness = s.brightness;
                OUT.uv = offset;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float distance = length(IN.uv);

                float alpha = smoothstep(1.0f, 0.0, distance);

                float3 ampBrightness = IN.brightness * 1.5f;

                return float4(ampBrightness, alpha);
            }
            ENDHLSL
        }
    }
}
