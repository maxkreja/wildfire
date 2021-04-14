using UnityEngine;

// [RequireComponent(typeof(MeshRenderer))]
public class Brush : MonoBehaviour
{
    [SerializeField]
    private Indicator indicator;

    [SerializeField]
    private LayerData layerData;
    [SerializeField]
    private TextureData textureData;

    [SerializeField]
    private float movementDelta = 0.01f;
    [SerializeField]
    private float heightOffset;
    [SerializeField]
    private float radius;
    [SerializeField]
    private float minRadius;
    [SerializeField]
    private float maxRadius;
    [SerializeField]
    private float radiusIncrement;
    [SerializeField]
    private int density;
    [SerializeField]
    private int minDensity;
    [SerializeField]
    private int maxDensity;

    [SerializeField]
    private GameObject plantParent;
    [SerializeField]
    private GameObject seedPreset;

    [SerializeField]
    private PlantData[] plantDatas;
    private int selectedPlant = 0;

    private bool brushEnabled = false;

    // private MeshRenderer meshRenderer;
    private Vector3 previousPosition;

    void Start()
    {
        // meshRenderer = GetComponent<MeshRenderer>();
        // meshRenderer.enabled = false;
        previousPosition = Vector3.zero;
        UpdateDensity(density);
        UpdateRadius(radius);
    }

    void Update()
    {
        // transform.localScale = new Vector3(radius * 2, transform.localScale.y, radius * 2);
    }

    public void UpdatePosition(Vector3 position, Vector3 normal)
    {
        transform.position = position;
        // transform.rotation = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
    }

    void UpdateRadius(float radius)
    {
        this.radius = radius;
        indicator.SetRadius(radius);
    }

    void UpdateDensity(int density)
    {
        this.density = density;
        indicator.SetDensity(density);
    }

    public void IncreaseRadius()
    {
        float target = radius + radiusIncrement;
        if (target > maxRadius)
        {
            if (radius != maxRadius)
            {
                UpdateRadius(maxRadius);
            }
        }
        else UpdateRadius(target);
    }

    public void DecreaseRadius()
    {
        float target = radius - radiusIncrement;
        if (target < minRadius)
        {
            if (radius != minRadius)
            {
                UpdateRadius(minRadius);
            }
        }
        else UpdateRadius(target);
    }

    public void IncreaseDensity()
    {
        int target = density + 1;
        if (target > maxDensity)
        {
            if (density != maxDensity)
            {
                UpdateDensity(maxDensity);
            }
        }
        else UpdateDensity(target);
    }

    public void DecreaseDensity()
    {
        int target = density - 1;
        if (target < minDensity)
        {
            if (density != minDensity)
            {
                UpdateDensity(minDensity);
            }
        }
        else UpdateDensity(target);
    }

    public void SetBrushEnabled(bool enable)
    {
        brushEnabled = enable;
        indicator.gameObject.SetActive(enable);
    }

    public void Paint()
    {
        if (Vector3.Distance(previousPosition, transform.position) > movementDelta)
        {
            previousPosition = transform.position;

            if (selectedPlant >= 0 && selectedPlant < plantDatas.Length)
            {
                GameObject[] seeds = new GameObject[density];
                for (int i = 0; i < density; i++)
                {
                    float r = radius * Mathf.Sqrt(Random.value);
                    float theta = Random.value * 2 * Mathf.PI;

                    float x = transform.position.x + r * Mathf.Cos(theta);
                    float z = transform.position.z + r * Mathf.Sin(theta);

                    Vector3 position = new Vector3(x, transform.position.y + heightOffset, z);
                    seeds[i] = Instantiate(seedPreset, position, Quaternion.identity);

                    if (plantParent != null)
                    {
                        seeds[i].transform.parent = plantParent.transform;
                    }
                }

                int offset = Random.Range(0, seeds.Length);
                for (int i = 0; i < seeds.Length; i++)
                {
                    Seed seed = seeds[(i + offset) % seeds.Length].GetComponent<Seed>();
                    seed.Grow(new PlantInstance(plantDatas[selectedPlant]), layerData, textureData);
                }
            }
        }
    }

    public int GetSelectedPlant()
    {
        return selectedPlant;
    }

    public void NextPlant()
    {
        if (selectedPlant + 1 == plantDatas.Length)
        {
            selectedPlant = 0;
        }
        else
        {
            selectedPlant = selectedPlant + 1;
        }
    }

    public void PreviousPlant()
    {
        if (selectedPlant == 0)
        {
            selectedPlant = plantDatas.Length - 1;
        }
        else
        {
            selectedPlant = selectedPlant - 1;
        }
    }

    public void SelectPlant(int index)
    {
        if (index >= 0 && index < plantDatas.Length)
        {
            selectedPlant = index;
        }
    }

    public int GetPlantCount()
    {
        return plantDatas.Length;
    }

    public Sprite[] GetIcons()
    {
        Sprite[] sprites = new Sprite[plantDatas.Length];
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i] = plantDatas[i].GetIcon();
        }

        return sprites;
    }
}
