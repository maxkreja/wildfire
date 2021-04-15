using UnityEngine;

public class Brush : MonoBehaviour
{
    [SerializeField]
    private LayerData layerData;
    [SerializeField]
    private TextureData textureData;

    [SerializeField]
    private Indicator indicator;

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
    private PlantData[] plantDatas;
    private int selectedPlant = 0;

    private bool brushEnabled = false;
    private Vector3 previousPosition;

    void Start()
    {
        previousPosition = Vector3.zero;
        UpdateDensity(density);
        UpdateRadius(radius);
    }

    public void UpdatePosition(Vector3 position, Vector3 normal)
    {
        transform.position = position;
    }

    public void IncreaseRadius()
    {
        float target = Mathf.Min(radius + radiusIncrement, maxRadius);
        UpdateRadius(target);
    }

    public void DecreaseRadius()
    {
        float target = Mathf.Max(radius - radiusIncrement, minRadius);
        UpdateRadius(target);
    }

    public void IncreaseDensity()
    {
        int target = Mathf.Min(density + 1, maxDensity);
        UpdateDensity(target);
    }

    public void DecreaseDensity()
    {
        int target = Mathf.Max(density - 1, minDensity);
        UpdateDensity(target);
    }

    public void SetBrushEnabled(bool enable)
    {
        brushEnabled = enable;
        indicator.gameObject.SetActive(enable);
    }

    public void Paint()
    {
        if (brushEnabled && Vector3.Distance(previousPosition, transform.position) > movementDelta)
        {
            previousPosition = transform.position;
            if (selectedPlant >= 0 && selectedPlant < plantDatas.Length)
            {
                for (int i = 0; i < density; i++)
                {
                    Vector2 point = RandomPointInCircle(radius, new Vector2(transform.position.x, transform.position.z));
                    Vector3 position = new Vector3(point.x, transform.position.y + heightOffset, point.y);

                    GameObject seedObject = new GameObject();
                    seedObject.transform.position = position;
                    seedObject.transform.rotation = Quaternion.identity;
                    seedObject.transform.parent = plantParent.transform;
                    Seed seed = seedObject.AddComponent<Seed>();

                    seed.Grow(new PlantInstance(plantDatas[selectedPlant]), layerData, textureData);
                }
            }
        }
    }

    public int GetSelectedPlant()
    {
        return selectedPlant;
    }

    public void SetSelectedPlant(int index)
    {
        if (index >= 0 && index < plantDatas.Length) selectedPlant = index;
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

    private void UpdateRadius(float radius)
    {
        this.radius = radius;
        indicator.SetRadius(radius);
    }

    private void UpdateDensity(int density)
    {
        this.density = density;
        indicator.SetDensity(density);
    }

    private static Vector2 RandomPointInCircle(float radius, Vector2 center)
    {
        float r = radius * Mathf.Sqrt(Random.value);
        float theta = Random.value * 2 * Mathf.PI;

        float x = center.x + r * Mathf.Cos(theta);
        float y = center.y + r * Mathf.Sin(theta);

        return new Vector2(x, y);
    }
}
