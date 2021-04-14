using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneratorSeed : MonoBehaviour
{
    private int seed;

    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void SetSeed(int seed)
    {
        this.seed = seed;
    }

    public int GetSeed()
    {
        return seed;
    }
}
