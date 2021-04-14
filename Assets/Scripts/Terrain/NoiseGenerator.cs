using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{
    [SerializeField]
    private float scale = 50;
    [SerializeField]
    private int octaves = 8;
    [SerializeField]
    private float persistance = 0.5f;
    [SerializeField]
    private float lacunarity = 2;

    [SerializeField]
    private int seed = 0;

    void OnValidate()
    {
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;
        persistance = Mathf.Clamp(persistance, 0, 1);
    }

    public float[,] Generate(int width, int height, Vector2 offset)
    {
        float[,] noiseMap = new float[width, height];

        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffset = new Vector2[octaves];

        float maxPossibleHeight = 0;
        float amplitude = 1;
        float frequency = 1;

        for (int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) - offset.y;

            octaveOffset[i] = new Vector2(offsetX, offsetY);

            maxPossibleHeight = maxPossibleHeight + amplitude;
            amplitude = amplitude * persistance;
        }

        if (scale <= 0) scale = float.Epsilon;

        float maxLocalNoiseHeight = float.MinValue;
        float minNoiseLocalHeight = float.MaxValue;

        float halfWidth = width / 2;
        float halfHeight = height / 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                amplitude = 1;
                frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth + octaveOffset[i].x) / scale * frequency;
                    float sampleY = (y - halfHeight + octaveOffset[i].y) / scale * frequency;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight = noiseHeight + perlinValue * amplitude;

                    amplitude = amplitude * persistance;
                    frequency = frequency * lacunarity;
                }

                if (noiseHeight > maxLocalNoiseHeight) maxLocalNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseLocalHeight) minNoiseLocalHeight = noiseHeight;

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseLocalHeight, maxLocalNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }

    public void SetSeed(int seed)
    {
        this.seed = seed;
    }
}
