using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshCollider))]
public class Plant : MonoBehaviour
{
    private LayerData layerData;
    private PlantData plantData;
    private PlantInstance plantInstance;

    private PlantState currentState;
    private PlantState previousState;

    private MeshCollider meshCollider;
    private GameObject currentMesh;

    private Fire fire;

    private float ignitionDeltaTime;
    private float keepAliveDeltaTime;

    private bool initialized;

    void Update()
    {
        if (initialized)
        {
            if (currentState == PlantState.Default && plantData.instantIgnite)
                Ignite(true);

            if (currentState == PlantState.Depleted)
            {
                keepAliveDeltaTime += Time.deltaTime;
                if (keepAliveDeltaTime > plantData.keepAlive)
                {
                    Destroy(gameObject);
                }
            }

            if (currentState != previousState)
            {
                previousState = currentState;
                UpdateDisplayMesh();
            }
        }
    }

    void FixedUpdate()
    {
        if (initialized)
        {
            if (currentState == PlantState.Burning)
            {
                ignitionDeltaTime = ignitionDeltaTime + Time.deltaTime;
                if (ignitionDeltaTime > plantData.tickLength)
                {
                    ignitionDeltaTime = 0;
                    IgnitePlants();
                }

                currentState = plantInstance.ConsumeFuel(Time.deltaTime);
            }
        }
    }

    public void Initialize(PlantInstance plantInstance, LayerData layerData)
    {
        this.plantInstance = plantInstance;
        this.plantData = this.plantInstance.GetPlantData();
        this.layerData = layerData;

        currentState = PlantState.Default;
        previousState = currentState;

        meshCollider = GetComponent<MeshCollider>();
        meshCollider.sharedMesh = plantData.collisionMesh;

        GameObject fireObject = Instantiate(plantData.firePrefab, transform.position, transform.rotation);
        fireObject.transform.parent = transform;
        fireObject.transform.localScale = Vector3.one;
        fire = fireObject.GetComponent<Fire>();

        fire.SetMesh(plantData.emissionMesh);

        UpdateDisplayMesh();
        initialized = true;
    }

    public void Ignite(bool force = false)
    {
        currentState = plantInstance.Ignite(force);
    }

    public bool IsIgnitable()
    {
        return !plantData.instantIgnite;
    }

    private void UpdateDisplayMesh()
    {
        Destroy(currentMesh);

        switch (currentState)
        {
            case PlantState.Default:
                currentMesh = Instantiate(plantData.defaultPrefab, transform.position, transform.rotation, transform);
                fire.Extinguish();
                break;
            case PlantState.Burning:
                currentMesh = Instantiate(plantData.burningPrefab, transform.position, transform.rotation, transform);
                fire.Ignite();
                break;
            case PlantState.Depleted:
                currentMesh = Instantiate(plantData.depletedPrefab, transform.position, transform.rotation, transform);
                fire.Extinguish();
                meshCollider.enabled = plantData.isCigarette || false;
                break;
        }

        currentMesh.transform.localScale = Vector3.one;
    }

    private void IgnitePlants()
    {
        meshCollider.enabled = false;
        Collider[] colliders = Physics.OverlapSphere(transform.position, plantData.ignitionDistance, layerData.plants.value);
        colliders = colliders.OrderBy((colliders) => Vector3.Distance(colliders.transform.position, transform.position)).ToArray();

        int ignitions = 0;
        for (int i = 0; i < colliders.Length; i++)
        {
            Collider collider = colliders[i];
            Debug.DrawLine(transform.position + Vector3.up, collider.transform.position + Vector3.up, Color.magenta, plantData.tickLength);
            GameObject gameObject = collider.transform.gameObject;
            Plant plant = gameObject.GetComponent<Plant>();

            if (plant != null && plant.IsIgnitable())
            {
                plant.Ignite();
                ignitions++;
            }

            if (ignitions == plantData.maxIgnitions) break;
        }

        meshCollider.enabled = true;
    }
}
