using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FalloffGenerator : MonoBehaviour
{
    [SerializeField]
    [Range(0, 10)]
    private float a = 3f;
    [SerializeField]
    [Range(0, 10)]
    private float b = 2.2f;

    public float[,] Generate(int width, int height)
    {
        float[,] map = new float[width, height];

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float x = i / (float)width * 2 - 1;
                float y = j / (float)height * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value);
            }
        }

        return map;
    }

    float Evaluate(float value)
    {
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}
