using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FalloffGenerator))]
public class FalloffGeneratorEditor : Editor
{
    private Texture2D texture;

    void OnEnable()
    {
        UpdatePreview();
    }

    public override void OnInspectorGUI()
    {
        if (texture != null) EditorGUILayout.ObjectField("Preview", texture, typeof(Texture2D), false);

        if (DrawDefaultInspector())
        {
            UpdatePreview();
        }
    }

    void UpdatePreview()
    {
        FalloffGenerator generator = (FalloffGenerator)target;
        float[,] noiseMap = generator.Generate(120, 120);
        texture = TextureGenerator.TextureFromHeightMap(noiseMap);
    }
}
