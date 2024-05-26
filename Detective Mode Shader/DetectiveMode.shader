
Shader "Hidden/DetectiveMode" {
    
    Properties {
        _MainTex ("Texture", 2D) = "white"
    }

    SubShader {
        Tags {
            "RenderType"="Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass {
            Name "DetectiveMode"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            float4 _MainTex_ST;

            SamplerState sampler_point_clamp;

            Varyings vert(Attributes IN) {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_TARGET {
                half4 tex = SAMPLE_TEXTURE2D(_MainTex, sampler_point_clamp, IN.uv);
                // Perform lerp operation on the sampled texture directly
                tex.rgb = lerp(tex.rgb, float3(0, 0, 1), 0.3);
                tex.a *= 0.3;
                return tex;
            }
            ENDHLSL
        }
    }
    
}
