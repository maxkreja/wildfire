using UnityEngine;

public class Seed : MonoBehaviour
{
    private LayerData layerData;

    private State state;
    private int plantLayer;

    private PlantData plantData;
    private PlantInstance plantInstance;

    private Plant plant;
    private GameObject plantObject;
    private GameObject seedVisual;

    private float growDelay;
    private float delta = 0;

    void Update()
    {
        switch (state)
        {
            case State.Growing:
                instantiatePlant();
                state = State.GrowingVisually;
                break;
            case State.GrowingVisually:
                plantObject.transform.localScale = plant.transform.localScale + Vector3.one * Time.deltaTime * plantData.growingSpeed;
                if (plant.transform.localScale.x >= 1)
                {
                    plant.transform.localScale = Vector3.one;
                    state = State.Grown;
                    plant.transform.parent = transform.parent;
                    Destroy(gameObject);
                }
                break;
        }
    }

    void FixedUpdate()
    {
        switch (state)
        {
            case State.Seed:
                delta += Time.deltaTime;
                if (delta > growDelay)
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
            case State.Grown:
                Destroy(GetComponent<MeshCollider>()); // disable mesh collider so rays collide with plant
                break;
            case State.Growing:
                GetComponent<MeshCollider>().convex = false; // make collider non convex for accurate collisions
                break;
            case State.TryGrowing:
                Destroy(GetComponent<Rigidbody>());
                state = State.Growing;
                break;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state == State.TryGrowing)
        {
            int layer = collision.gameObject.layer;
            if (layer == plantLayer)
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
        plantLayer = (int)Mathf.Log(layerData.plants.value, 2);
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

                gameObject.layer = plantLayer;

                if (plantData.seedPrefab != null)
                {
                    seedVisual = Instantiate(plantData.seedPrefab, transform.position, transform.rotation);
                    seedVisual.transform.parent = transform;
                }

                float scale = plantInstance.GetScale();
                transform.localScale = new Vector3(scale, scale, scale);

                if (!plantData.instantIgnite && !plantData.isCigarette)
                {
                    setupSaturation();
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

    private void instantiatePlant()
    {
        if (seedVisual != null)
        {
            Destroy(seedVisual);
        }

        plantObject = new GameObject();
        plantObject.layer = plantLayer;
        plantObject.AddComponent<MeshCollider>();

        float scale = plantInstance.GetScale();
        plantObject.transform.position = transform.position;
        plantObject.transform.localScale = Vector3.zero;
        plantObject.transform.rotation = transform.rotation;
        plantObject.transform.parent = transform;

        plant = plantObject.AddComponent<Plant>();
        plant.Initialize(plantInstance, layerData);
    }

    private void setupSaturation()
    {
        Vector3 origin = transform.position + Vector3.up;
        int iterations = plantData.waterCheckIterations;
        int rays = plantData.waterCheckRays;
        int waterLayer = (int)Mathf.Log(layerData.water, 2);
        float radius = plantData.waterRadius;
        float increment = radius / iterations;
        float radians = (360 * Mathf.Deg2Rad) / rays;

        float distance = plantData.waterRadius;
        for (int i = 0; i < iterations; i++)
        {
            float offset = increment + i * increment;
            bool exit = false;

            for (int j = 0; j < rays; j++)
            {
                float x = Mathf.Cos(j * radians);
                float y = Mathf.Sin(j * radians);

                Vector3 target = new Vector3(origin.x + x * offset, transform.position.y, origin.z + y * offset);
                Vector3 direction = (target - origin).normalized;

                RaycastHit hit;
                if (Physics.Raycast(origin, direction, out hit, radius, layerData.terrain | layerData.water))
                {
                    if (hit.transform.gameObject.layer == waterLayer)
                    {
                        Vector2 a = new Vector2(origin.x, origin.y);
                        Vector2 b = new Vector2(hit.point.x, hit.point.y);
                        float pointDistance = Vector2.Distance(a, b);
                        if (pointDistance < distance)
                        {
                            distance = pointDistance;
                            exit = true;
                        }
                    }
                }
            }

            if (exit)
            {
                break;
            }
        }

        plantInstance.UpdateSaturation(distance);
    }

    private enum State
    {
        Seed,
        TryGrowing,
        Growing,
        GrowingVisually,
        Grown
    }
}
