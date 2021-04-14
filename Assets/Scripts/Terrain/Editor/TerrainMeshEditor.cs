using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainMesh))]
public class TerrainMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (DrawDefaultInspector())
        {
            if (GUILayout.Button("Generate"))
            {
                TerrainMesh terrain = (TerrainMesh)target;
                terrain.Generate();
            }
        }
    }
}
