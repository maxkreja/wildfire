using System;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField]
    private NoiseGenerator noiseGenerator;
    [SerializeField]
    private FalloffGenerator falloffGenerator;

    public int mapWidth = 95;
    public int mapHeight = 95;

    [SerializeField]
    private Vector2 offset;
    [SerializeField]
    private float heightMultiplier = 1;
    [SerializeField]
    private AnimationCurve heightCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [SerializeField]
    private Material terrainMaterial;
    [SerializeField]
    private TextureData textureData;

    private float maxNoiseHeight = -Mathf.Infinity;

    private float minHeight { get { return transform.position.y; } }
    // private float maxHeight { get { return transform.localScale.y * heightMultiplier * heightCurve.Evaluate(1) * maxNoiseHeight; } }
    private float maxHeight = 1;

    private Queue<MapThreadInfo<MapData>> mapDataQueue = new Queue<MapThreadInfo<MapData>>();
    private Queue<MapThreadInfo<MeshData>> meshDataQueue = new Queue<MapThreadInfo<MeshData>>();

    void OnValidate()
    {
        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureDataUpdated;
            textureData.OnValuesUpdated += OnTextureDataUpdated;
        }
    }

    void OnTextureDataUpdated()
    {
        textureData.UpdateData(terrainMaterial, minHeight, maxHeight);
    }

    void Update()
    {
        if (mapDataQueue.Count > 0)
        {
            for (int i = 0; i < mapDataQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }

        if (meshDataQueue.Count > 0)
        {
            for (int i = 0; i < meshDataQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    public void RequestMapData(Vector2 center, Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(center, callback);
        };

        new Thread(threadStart).Start();
    }

    void MapDataThread(Vector2 center, Action<MapData> callback)
    {
        MapData data = GenerateMapData(center);
        lock (mapDataQueue) mapDataQueue.Enqueue(new MapThreadInfo<MapData>(callback, data));
    }

    public void RequestMeshData(MapData mapData, Action<MeshData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, callback);
        };

        new Thread(threadStart).Start();
    }

    void MeshDataThread(MapData mapData, Action<MeshData> callback)
    {
        MeshData data = MeshGenerator.GenerateTerrainMesh(mapData.heightMap, heightMultiplier, heightCurve);
        lock (meshDataQueue) meshDataQueue.Enqueue(new MapThreadInfo<MeshData>(callback, data));
    }

    public MapData GenerateMapData(Vector2 center)
    {
        GameObject seedObject = GameObject.Find("Map Generator Seed");
        if (seedObject != null)
        {
            MapGeneratorSeed mapGeneratorSeed = seedObject.GetComponent<MapGeneratorSeed>();
            noiseGenerator.SetSeed(mapGeneratorSeed.GetSeed());
        }

        float[,] noiseMap = noiseGenerator.Generate(mapWidth, mapHeight, center + offset);
        float[,] falloffMap = new float[0, 0];

        bool useFalloff = falloffGenerator != null;
        if (useFalloff) falloffMap = falloffGenerator.Generate(mapWidth, mapHeight);

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if (useFalloff) noiseMap[x, y] = Mathf.Clamp(noiseMap[x, y] - falloffMap[x, y], 0, 1);

                float currentHeight = noiseMap[x, y];
                if (currentHeight > maxNoiseHeight) maxNoiseHeight = currentHeight;
            }
        }

        maxHeight = transform.localScale.y * heightMultiplier * heightCurve.Evaluate(1) * maxNoiseHeight;
        textureData.UpdateData(terrainMaterial, minHeight, maxHeight);

        return new MapData(noiseMap);
    }

    public MeshData GenerateMeshData(MapData mapData)
    {
        return MeshGenerator.GenerateTerrainMesh(mapData.heightMap, heightMultiplier, heightCurve);
    }

    struct MapThreadInfo<T>
    {
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

public struct MapData
{
    public readonly float[,] heightMap;

    public MapData(float[,] heightMap)
    {
        this.heightMap = heightMap;
    }
}
