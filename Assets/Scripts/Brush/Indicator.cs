using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{
    [SerializeField]
    private LayerData layerData;

    [SerializeField]
    private GameObject dotPrefab;
    [SerializeField]
    private GameObject densityIndicator;

    [SerializeField]
    private float densityFactor = 1f;

    [SerializeField]
    private int dotCount;

    private GameObject[] dots;

    private float radius = 1f;
    private float dotDistance;
    private int density;
    private float densityIndicatorBaseScale;

    void Start()
    {
        dots = new GameObject[0];
        densityIndicatorBaseScale = densityIndicator.transform.localScale.x;
    }

    void Update()
    {
        densityIndicator.transform.localScale = Vector3.one * densityIndicatorBaseScale * density * densityFactor;
        UpdateDots();
    }

    public void SetDensity(int density)
    {
        this.density = density;
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
    }

    private void ClearDots()
    {
        for (int i = 0; i < dots.Length; i++)
        {
            Destroy(dots[i]);
        }
    }

    private void UpdateDots()
    {
        float radians = (360 * Mathf.Deg2Rad) / dotCount;

        if (dots.Length != dotCount)
        {
            ClearDots();
            dots = new GameObject[dotCount];

            for (int i = 0; i < dots.Length; i++)
            {
                dots[i] = Instantiate(dotPrefab);
                dots[i].transform.parent = transform;
            }
        }

        for (int i = 0; i < dots.Length; i++)
        {
            float x = Mathf.Cos(i * radians);
            float y = Mathf.Sin(i * radians);

            Vector3 position = new Vector3(transform.position.x + x * radius, 100, transform.position.z + y * radius);
            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.down, out hit, Mathf.Infinity, layerData.terrain.value))
            {
                position.y = hit.point.y;
            }
            else position.y = transform.position.y;

            dots[i].transform.position = position;
        }
    }
}
