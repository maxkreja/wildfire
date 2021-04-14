using UnityEngine;
using System.Linq;

[RequireComponent(typeof(MeshCollider))]
public class Plant : MonoBehaviour
{
    private PlantInstance.PlantState currentState;
    private PlantInstance.PlantState previousState;

    private MeshCollider meshCollider;
    private GameObject prefab;

    private Fire fire;

    private float ignitionDelta;
    private float keepAliveDelta;

    private LayerData layerData;

    private PlantData plantData;
    private PlantInstance plantInstance;

    private InitState initState = InitState.Default;

    void Update()
    {
        if (initState == InitState.Initialized && currentState == PlantInstance.PlantState.Default)
        {
            if (plantData.instantIgnite) Ignite(true);
        }

        if (initState == InitState.Initialize)
        {
            currentState = PlantInstance.PlantState.Default;
            previousState = currentState;

            meshCollider = GetComponent<MeshCollider>();
            meshCollider.sharedMesh = plantData.collisionMesh;

            GameObject fireObject = Instantiate(plantData.firePrefab, transform.position, transform.rotation);
            fireObject.transform.parent = transform;
            fireObject.transform.localScale = Vector3.one;
            fire = fireObject.GetComponent<Fire>();

            fire.SetMesh(plantData.emissionMesh);

            initState = InitState.Initialized;

            updateDisplayMesh();
        }

        if (currentState != previousState)
        {
            previousState = currentState;
            updateDisplayMesh();
        }

        if (currentState == PlantInstance.PlantState.Depleted)
        {
            keepAliveDelta += Time.deltaTime;
            if (keepAliveDelta > plantData.keepAlive)
            {
                Destroy(gameObject);
            }
        }
    }

    void FixedUpdate()
    {
        if (initState == InitState.Initialized)
        {
            if (currentState == PlantInstance.PlantState.Burning)
            {
                ignitionDelta = ignitionDelta + Time.deltaTime;
                if (ignitionDelta > plantData.tickLength)
                {
                    ignitionDelta = 0;
                    ignitePlants();
                }

                currentState = plantInstance.ConsumeFuel(Time.deltaTime);
            }
        }
    }

    void OnDrawGizmos()
    {
        if (initialized())
        {
            Gizmos.DrawWireSphere(transform.position, plantData.ignitionDistance);
        }
    }

    public void Initialize(PlantInstance plantInstance, LayerData layerData)
    {
        this.plantInstance = plantInstance;
        this.plantData = this.plantInstance.GetPlantData();
        this.layerData = layerData;
        initState = InitState.Initialize;
    }

    public void Ignite(bool force = false)
    {
        currentState = plantInstance.Ignite(force);
    }

    public bool IsIgnitable()
    {
        return !plantData.instantIgnite;
    }

    private void updateDisplayMesh()
    {
        Destroy(prefab);

        switch (currentState)
        {
            case PlantInstance.PlantState.Default:
                prefab = Instantiate(plantData.defaultPrefab, transform.position, transform.rotation);
                fire.Extinguish();
                break;
            case PlantInstance.PlantState.Burning:
                prefab = Instantiate(plantData.burningPrefab, transform.position, transform.rotation);
                fire.Ignite();
                break;
            case PlantInstance.PlantState.Depleted:
                prefab = Instantiate(plantData.depletedPrefab, transform.position, transform.rotation);
                fire.Extinguish();
                meshCollider.enabled = plantData.isCigarette || false;
                break;
        }

        prefab.transform.parent = transform;
        prefab.transform.localScale = Vector3.one;
    }

    private void ignitePlants()
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

            if (ignitions == plantData.maxIgnitions)
            {
                break;
            }
        }

        meshCollider.enabled = true;
    }

    private bool initialized()
    {
        return plantData != null && plantInstance != null && layerData != null;
    }

    private enum InitState
    {
        Default,
        Initialize,
        Initialized,
    }
}
