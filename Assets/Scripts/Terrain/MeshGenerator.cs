using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve)
    {
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);

        int mapWidth = heightMap.GetLength(0);
        int mapHeight = heightMap.GetLength(1);

        float topLeftX = (mapWidth - 1) / -2f;
        float topLeftZ = (mapHeight - 1) / 2f;

        MeshData meshData = new MeshData(mapWidth, mapHeight);
        int vertexIndex = 0;

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float height = heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier;

                Vector2 uv = new Vector2(x / (float)mapWidth, y / (float)mapHeight);
                Vector3 position = new Vector3(topLeftX + x, height, topLeftZ - y);

                meshData.AddVertex(position, uv, vertexIndex);

                if (x < mapWidth - 1 && y < mapHeight - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + mapWidth + 1, vertexIndex + mapWidth);
                    meshData.AddTriangle(vertexIndex + mapWidth + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex = vertexIndex + 1;
            }
        }

        meshData.ProcessMesh();
        return meshData;
    }
}

public class MeshData
{
    Vector3[] vertices;
    int[] triangles;

    Vector2[] uvs;
    Vector3[] bakedNormals;

    int triangleIndex;

    public Vector3[] GetVertices()
    {
        return this.vertices;
    }

    public MeshData(int width, int height)
    {
        vertices = new Vector3[width * height];
        triangles = new int[(width - 1) * (height - 1) * 6];
        uvs = new Vector2[width * height];
    }

    public void AddVertex(Vector3 position, Vector2 uv, int vertexIndex)
    {
        vertices[vertexIndex] = position;
        uvs[vertexIndex] = uv;
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex = triangleIndex + 3;
    }

    Vector3[] BakeNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];

        int triangleCount = triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormaleFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i] = vertexNormals[i].normalized;
        }

        return vertexNormals;
    }

    Vector3 SurfaceNormaleFromIndices(int indexA, int indexB, int indexC)
    {
        Vector3 pointA = vertices[indexA];
        Vector3 pointB = vertices[indexB];
        Vector3 pointC = vertices[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;

        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public void ProcessMesh()
    {
        FlatShading();
        // bakedNormals = BakeNormals();
    }

    void FlatShading()
    {
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];
        Vector2[] flatShadedUvs = new Vector2[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUvs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }

        vertices = flatShadedVertices;
        uvs = flatShadedUvs;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();
        // mesh.normals = bakedNormals;

        return mesh;
    }
}
