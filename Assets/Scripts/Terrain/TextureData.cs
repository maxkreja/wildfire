using UnityEngine;
using System.Linq;

[CreateAssetMenu()]
public class TextureData : ScriptableObject
{
    const TextureFormat textureFormat = TextureFormat.RGB565;

    public event System.Action OnValuesUpdated;
    public bool autoUpdate;

    public int textureSize = 64;

    public Layer[] layers;

    private float minHeight;
    private float maxHeight;

#if UNITY_EDITOR
    protected virtual void OnValidate()
    {
        if (autoUpdate) UnityEditor.EditorApplication.update += NotifyOfUpdatedValues;
    }

    public void NotifyOfUpdatedValues()
    {
        UnityEditor.EditorApplication.update -= NotifyOfUpdatedValues;
        if (OnValuesUpdated != null) OnValuesUpdated();
    }
#endif

    public void UpdateData(Material material, float minHeight, float maxHeight)
    {
        this.minHeight = minHeight;
        this.maxHeight = maxHeight;

        material.SetInt("layerCount", layers.Length);
        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
        material.SetColorArray("baseColors", layers.Select(x => x.tint).ToArray());
        material.SetFloatArray("baseStartHeights", layers.Select(x => x.startHeight).ToArray());
        material.SetFloatArray("baseBlends", layers.Select(x => x.blendStrength).ToArray());
        material.SetFloatArray("baseColorStrength", layers.Select(x => x.tintStrength).ToArray());
        material.SetFloatArray("baseTextureScales", layers.Select(x => x.textureScale).ToArray());

        Texture2D[] textures = layers.Select(x => x.texture).ToArray();
        Texture2DArray textureArray = new Texture2DArray(textureSize, textureSize, layers.Length, textureFormat, true);
        for (int i = 0; i < textures.Length; i++)
        {
            textureArray.SetPixels(textures[i].GetPixels(), i);
        }
        textureArray.Apply();

        material.SetTexture("baseTextures", textureArray);
    }

    private float InverseLerp(float a, float b, float value)
    {
        return Mathf.Clamp((value - a) / (b - a), 0, 1);
    }

    public bool FoliageAllowed(Vector3 point)
    {
        float height = InverseLerp(minHeight, maxHeight, point.y);

        for (int i = 0; i < layers.Length; i++)
        {
            if (layers[i].allowFoliage)
            {
                float low = layers[i].startHeight;

                if (i + 1 < layers.Length)
                {
                    float high = layers[i + 1].startHeight;
                    if (height >= low && height <= high) return true;
                }
                else if (height >= low) return true;
            }
        }

        return false;
    }

    [System.Serializable]
    public class Layer
    {
        public Texture2D texture;
        public Color tint;
        [Range(0, 1)]
        public float tintStrength;
        [Range(0, 1)]
        public float startHeight;
        [Range(0, 1)]
        public float blendStrength;
        public float textureScale = 1;
        public bool allowFoliage = false;
    }
}
