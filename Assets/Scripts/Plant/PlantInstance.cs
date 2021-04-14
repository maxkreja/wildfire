using UnityEngine;

public class PlantInstance
{
    private PlantData data;

    private float fuel;
    private float scale;
    private float burnRate;
    private float saturation = 0;

    private PlantState state;

    public PlantInstance(PlantData plantData)
    {
        data = plantData;

        fuel = plantData.GetInitialFuel();
        scale = plantData.GetScale(fuel);
        burnRate = plantData.GetBurnRate();

        state = PlantState.Default;
    }

    public PlantData GetPlantData()
    {
        return data;
    }

    public float GetScale()
    {
        return scale;
    }

    public PlantState ConsumeFuel(float deltaTime)
    {
        fuel = fuel - (burnRate * deltaTime);
        if (fuel <= 0)
        {
            fuel = 0;
            state = PlantState.Depleted;
        }

        return state;
    }

    public PlantState Ignite(bool force = false)
    {

        if (state == PlantState.Default)
        {
            if (force) state = PlantState.Burning;
            else
            {
                float probability = data.ignitionProbability * (1f - saturation);
                float random = Random.value;

                if (random <= probability)
                {
                    state = PlantState.Burning;
                }
            }
        }

        return state;
    }

    public void UpdateSaturation(float waterDistance)
    {
        saturation = 1 - waterDistance / data.waterRadius;
    }
}

public enum PlantState
{
    Default,
    Burning,
    Depleted
}
