using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneratorSeed : MonoBehaviour
{
    public static readonly string objectName = "Map Generator Seed";
    [SerializeField]
    private int seed;

    public void SetSeed(int seed)
    {
        this.seed = seed;
    }

    public int GetSeed()
    {
        return seed;
    }
}
