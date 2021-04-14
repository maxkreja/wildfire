using UnityEngine;

public class Seed : MonoBehaviour
{
    private LayerData layerData;
    private int plantLayerIndex;

    private State state;

    private Plant plant;
    private PlantData plantData;
    private PlantInstance plantInstance;

    private GameObject plantObject;
    private GameObject seedVisual;

    private float growDelay;
    private float deltaTime = 0;

    void Update()
    {
        switch (state)
        {
            case State.GrowingAllowed:
                InstantiatePlant();
                state = State.GrowingVisually;
                break;
            case State.GrowingVisually:
                UpdatePlantScale();
                break;
        }
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.Seed:
                deltaTime += Time.deltaTime;
                if (deltaTime > growDelay)
                {
                    Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
                    MeshCollider collider = gameObject.AddComponent<MeshCollider>();

                    collider.convex = true;

                    rigidbody.useGravity = false;
                    rigidbody.freezeRotation = true;
                    rigidbody.constraints = RigidbodyConstraints.FreezeAll;

                    collider.sharedMesh = plantData.collisionMesh;
                    state = State.TryGrowing;
                }
                break;
            case State.GrowingAllowed:
                GetComponent<MeshCollider>().convex = false; // make collider non convex for accurate collisions
                break;
            case State.TryGrowing:
                Destroy(GetComponent<Rigidbody>());
                state = State.GrowingAllowed;
                break;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state == State.TryGrowing)
        {
            int layer = collision.gameObject.layer;
            if (layer == plantLayerIndex)
            {
                Destroy(gameObject);
            }
        }
    }

    public void Grow(PlantInstance plantInstance, LayerData layerData, TextureData textureData)
    {
        this.plantInstance = plantInstance;
        this.layerData = layerData;

        plantData = this.plantInstance.GetPlantData();
        plantLayerIndex = LayerData.MaskToIndex(layerData.plants);
        growDelay = Random.Range(0f, 0.5f);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, layerData.terrain.value))
        {
            if (textureData.FoliageAllowed(hit.point))
            {
                transform.position = hit.point;
                transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;

                if (plantData.randomRotation)
                {
                    transform.RotateAround(transform.position, transform.up, Random.Range(0f, 360f));
                }

                gameObject.layer = plantLayerIndex;

                if (plantData.seedPrefab != null)
                {
                    seedVisual = Instantiate(plantData.seedPrefab, transform.position, transform.rotation);
                    seedVisual.transform.parent = transform;
                }

                float scale = plantInstance.GetScale();
                transform.localScale = new Vector3(scale, scale, scale);

                if (!plantData.instantIgnite && !plantData.isCigarette)
                {
                    SetupSaturation();
                }

                state = State.Seed;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void UpdatePlantScale()
    {
        plantObject.transform.localScale = plant.transform.localScale + Vector3.one * Time.deltaTime * plantData.growingSpeed;
        if (plant.transform.localScale.x >= 1)
        {
            plant.transform.localScale = Vector3.one;
            plant.transform.parent = transform.parent;
            Destroy(gameObject);
        }
    }

    private void InstantiatePlant()
    {
        if (seedVisual != null) Destroy(seedVisual);

        plantObject = new GameObject();
        plantObject.transform.position = transform.position;
        plantObject.transform.rotation = transform.rotation;
        plantObject.transform.parent = transform;
        plantObject.transform.localScale = Vector3.zero;
        plantObject.layer = plantLayerIndex;
        plantObject.AddComponent<MeshCollider>();
        plant = plantObject.AddComponent<Plant>();
        plant.Initialize(plantInstance, layerData);
    }

    private void SetupSaturation()
    {
        Vector3 origin = transform.position + Vector3.up;

        int iterations = plantData.waterCheckIterations;
        int rays = plantData.waterCheckRays;

        int waterLayerIndex = LayerData.MaskToIndex(layerData.water);

        float radius = plantData.waterRadius;
        float increment = radius / iterations;
        float radians = (360 * Mathf.Deg2Rad) / rays;

        float distance = plantData.waterRadius;
        for (int i = 0; i < iterations; i++)
        {
            float offset = increment + i * increment;
            bool breakEarly = false;

            for (int j = 0; j < rays; j++)
            {
                float x = Mathf.Cos(j * radians);
                float y = Mathf.Sin(j * radians);

                Vector3 target = new Vector3(origin.x + x * offset, transform.position.y, origin.z + y * offset);
                Vector3 direction = target - origin;

                float rayDistance = GetRayDistance(origin, direction, waterLayerIndex);
                if (rayDistance < distance)
                {
                    distance = rayDistance;
                    breakEarly = true;
                }
            }

            if (breakEarly) break;
        }

        plantInstance.UpdateSaturation(distance);
    }

    private float GetRayDistance(Vector3 origin, Vector3 direction, int waterLayerIndex)
    {
        float distance = plantData.waterRadius;

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, Mathf.Infinity, layerData.terrain | layerData.water))
        {
            if (hit.transform.gameObject.layer == waterLayerIndex)
            {
                Vector2 a = new Vector2(origin.x, origin.y);
                Vector2 b = new Vector2(hit.point.x, hit.point.y);
                distance = Vector2.Distance(a, b); ;
            }
        }

        return distance;
    }

    private enum State
    {
        Seed,
        TryGrowing,
        GrowingAllowed,
        GrowingVisually
    }
}
