Shader "Custom/SimpleGodRay_FinalFix"
{
    Properties
    {
        [HDR] _Color ("Color (HDR)", Color) = (1, 1, 0.8, 1)
        _MainTex ("Noise Texture", 2D) = "white" {}
        _Speed ("Flow Speed (X, Y)", Vector) = (0, -0.2, 0, 0)
        _FadePower ("Top-Bottom Fade", Range(0.1, 10)) = 2.0
        _FresnelPower ("Edge Softness", Range(0, 5)) = 1.0
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent" 
            "RenderPipeline" = "UniversalPipeline"
        }


        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normalWS : NORMAL;
                float3 viewDirWS : TEXCOORD3;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _MainTex_ST;
                float4 _Speed;
                float _FadePower;
                float _FresnelPower;
            CBUFFER_END

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float3 positionWS = TransformObjectToWorld(v.vertex);
                o.normalWS = TransformObjectToWorldNormal(v.normal);
                o.viewDirWS = GetWorldSpaceViewDir(positionWS);
                
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
   
                float2 uvOffset = i.uv + _Time.y * _Speed.xy;
                half4 noise = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvOffset);
                

                float gradient = pow(abs(i.uv.y), _FadePower);

              
                float NdotV = saturate(dot(i.normalWS, normalize(i.viewDirWS)));
                float fresnel = pow(NdotV, _FresnelPower);

   
                float finalAlpha = _Color.a * noise.r * gradient * fresnel;

         
                finalAlpha = saturate(finalAlpha);

                return float4(_Color.rgb, finalAlpha);
            }
            ENDHLSL
        }
    }
}