using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NoiseGenerator))]
public class NoiseGeneratorEditor : Editor
{
    private Texture2D texture;

    void OnEnable()
    {
        UpdatePreview();
    }

    public override void OnInspectorGUI()
    {
        if (texture != null) EditorGUILayout.ObjectField("Preview", texture, typeof(Texture2D), false);

        DrawDefaultInspector();
        UpdatePreview();
    }

    void UpdatePreview()
    {
        NoiseGenerator generator = (NoiseGenerator)target;
        float[,] noiseMap = generator.Generate(120, 120, Vector2.zero);
        texture = TextureGenerator.TextureFromHeightMap(noiseMap);
    }
}
