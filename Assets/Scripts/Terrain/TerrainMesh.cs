using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class TerrainMesh : MonoBehaviour
{
    [SerializeField]
    private MapGenerator mapGenerator;

    public void Generate()
    {
        if (mapGenerator != null)
        {
            MeshFilter meshFilter = GetComponent<MeshFilter>();
            MeshCollider meshCollider = GetComponent<MeshCollider>();

            MapData mapData = mapGenerator.GenerateMapData(Vector2.zero);
            MeshData meshData = mapGenerator.GenerateMeshData(mapData);

            Mesh mesh = meshData.CreateMesh();

            meshFilter.sharedMesh = mesh;
            if (meshCollider != null) meshCollider.sharedMesh = mesh;
        }
    }
}
