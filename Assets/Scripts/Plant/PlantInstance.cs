using UnityEngine;

public class PlantInstance
{
    private PlantData data;

    private float fuel;
    private float scale;
    private float burnRate;
    private float saturation = 0;

    private PlantState currentState;
    private PlantState previousState;

    public PlantInstance(PlantData plantData)
    {
        data = plantData;

        fuel = plantData.GetInitialFuel();
        scale = plantData.GetScale(fuel);
        burnRate = plantData.GetBurnRate();

        currentState = PlantState.Default;
        previousState = currentState;
    }

    public PlantData GetPlantData()
    {
        return data;
    }

    public float GetScale()
    {
        return scale;
    }

    public void ConsumeFuel(float deltaTime)
    {
        fuel = fuel - (burnRate * deltaTime);
        if (fuel <= 0)
        {
            fuel = 0;
            currentState = PlantState.Depleted;
        }
    }

    public void Ignite(bool force = false)
    {

        if (currentState == PlantState.Default)
        {
            if (force) currentState = PlantState.Burning;
            else
            {
                float probability = data.ignitionProbability * (1f - saturation);
                float random = Random.value;

                if (random <= probability)
                {
                    currentState = PlantState.Burning;
                }
            }
        }
    }

    public void UpdateSaturation(float waterDistance)
    {
        saturation = 1 - waterDistance / data.waterRadius;
    }

    public PlantState GetCurrentState()
    {
        return currentState;
    }

    public bool DidStateChange()
    {
        bool result = currentState != previousState;
        if (result) previousState = currentState;
        return result;
    }
}

public enum PlantState
{
    Default,
    Burning,
    Depleted
}
