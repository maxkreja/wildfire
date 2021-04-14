using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Plant))]
public class PlantEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (GUILayout.Button("Ignite"))
        {
            Plant plant = (Plant)target;
            plant.Ignite(true);
        }
        DrawDefaultInspector();
    }
}
