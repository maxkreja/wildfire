Shader "Custom/Terrain Shader"
{
    Properties
    {
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        const static int maxLayerCount = 8;
        const static float epsilon = 1E-5;

        int layerCount;

        float minHeight;
        float maxHeight;

        float3 baseColors[maxLayerCount];
        float baseStartHeights[maxLayerCount];
        float baseBlends[maxLayerCount];
        float baseColorStrength[maxLayerCount];
        float baseTextureScales[maxLayerCount];

        UNITY_DECLARE_TEX2DARRAY(baseTextures);

        half _Glossiness;

        struct Input
        {
            float3 worldPos;
            float3 worldNormal;
        };

        float inverseLerp(float a, float b, float value) {
            return saturate((value - a) / (b - a));
        }

        float3 triplanar(float3 worldPos, float scale, float3 blendAxis, int textureIndex) {
            float3 scaledWorldPos = worldPos / scale;

            float3 x = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.y, scaledWorldPos.z, textureIndex)) * blendAxis.x;
            float3 y = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.z, textureIndex)) * blendAxis.y;
            float3 z = UNITY_SAMPLE_TEX2DARRAY(baseTextures, float3(scaledWorldPos.x, scaledWorldPos.y, textureIndex)) * blendAxis.z;

            return x + y + z;
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float height = inverseLerp(minHeight - epsilon, maxHeight, IN.worldPos.y);

            float3 blendAxis = abs(IN.worldNormal);
            blendAxis /= blendAxis.x + blendAxis.y + blendAxis.z;

            for (int i = 0; i < layerCount; i++) {
                float drawStrength = inverseLerp(-baseBlends[i] / 2 - epsilon, baseBlends[i] / 2, height - baseStartHeights[i]);

                float3 baseColor = baseColors[i] * baseColorStrength[i];
                float3 textureColor = triplanar(IN.worldPos, baseTextureScales[i], blendAxis, i) * (1 - baseColorStrength[i]);

                o.Albedo = o.Albedo * (1 - drawStrength) + (baseColor + textureColor) * drawStrength;
            }

            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
