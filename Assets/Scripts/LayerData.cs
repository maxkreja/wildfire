using UnityEngine;

[CreateAssetMenu()]
public class LayerData : ScriptableObject
{
    public LayerMask terrain;
    public LayerMask water;
    public LayerMask plants;

    public static int MaskToIndex(LayerMask layerMask)
    {
        return (int)Mathf.Log(layerMask.value, 2);
    }
}
