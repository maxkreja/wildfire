using UnityEngine;

[CreateAssetMenu()]
public class PlantData : ScriptableObject
{
    [SerializeField]
    [Range(0, 10)]
    private float fuel;
    [SerializeField]
    [Range(0, 10)]
    private float fuelRandomness;
    [SerializeField]
    [Range(0, 10)]
    private float burnRate;
    [SerializeField]
    [Range(0, 10)]
    private float burnRateRandomness;

    [Range(0, 1)]
    public float ignitionProbability;

    [Range(0, 2)]
    public float minScale;
    [Range(0, 2)]
    public float maxScale = 1;
    public bool randomRotation = true;

    public float waterRadius = 5f;
    public int waterCheckRays = 5;
    public int waterCheckIterations = 5;

    public float ignitionDistance = 5f;
    public float tickLength = 0.5f;
    public int maxIgnitions = 3;
    public float growingSpeed = 1f;
    public float keepAlive = 5f;

    public Mesh collisionMesh;
    public Mesh emissionMesh;
    public GameObject defaultPrefab;
    public GameObject burningPrefab;
    public GameObject depletedPrefab;
    public GameObject firePrefab;
    public GameObject seedPrefab;

    [SerializeField]
    private Sprite icon;

    public bool instantIgnite;
    public bool isCigarette;

    public float GetInitialFuel()
    {
        return fuel + Random.Range(-fuelRandomness, fuelRandomness);
    }

    public float GetBurnRate()
    {
        return burnRate + Random.Range(-burnRateRandomness, burnRateRandomness);
    }

    public float GetScale(float fuel)
    {
        if (minScale == maxScale)
        {
            return minScale;
        }
        else
        {
            float a = this.fuel - fuelRandomness;
            float b = this.fuel + fuelRandomness;

            return (maxScale - minScale) * ((fuel - a) / (b - a)) + minScale;
        }
    }

    public Sprite GetIcon()
    {
        return icon;
    }
}
