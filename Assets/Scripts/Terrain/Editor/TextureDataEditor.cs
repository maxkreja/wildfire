using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TextureData), true)]
public class TextureDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TextureData data = (TextureData)target;
        if (GUILayout.Button("Update"))
        {
            data.NotifyOfUpdatedValues();
            EditorUtility.SetDirty(target);
        }
    }
}