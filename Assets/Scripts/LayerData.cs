using UnityEngine;

[CreateAssetMenu()]
public class LayerData : ScriptableObject
{
    public LayerMask terrain;
    public LayerMask water;
    public LayerMask plants;
}
