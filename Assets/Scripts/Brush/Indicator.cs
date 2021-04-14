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
    public float densityFactor = 1f;

    [SerializeField]
    private int dotCount;

    private GameObject[] dots;

    private float radius = -1f;
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
        if (radius != -1f)
        {
            float radians = (360 * Mathf.Deg2Rad) / dotCount;

            float firstX = Mathf.Cos(radians);
            float firstY = Mathf.Sin(radians);

            Vector2 a = new Vector2(firstX * radius, firstY * radius);
            Vector2 b = new Vector2(radius, 0);

            dotDistance = Vector2.Distance(a, b);

            densityIndicator.transform.localScale = Vector3.one * densityIndicatorBaseScale * density * densityFactor;

            if (dotDistance < dotPrefab.transform.localScale.x)
            {
                if (dots.Length != 1)
                {
                    ClearDots();
                    dots = new GameObject[1];
                    dots[0] = Instantiate(dotPrefab);
                    dots[0].transform.parent = transform;
                }

                dots[0].transform.position = transform.position;
                dots[0].transform.localScale = new Vector3(radius * 2, dots[0].transform.localScale.y, radius * 2);
            }
            else
            {
                if (dots.Length != dotCount)
                {
                    ClearDots();

                    dots = new GameObject[dotCount];
                    for (int i = 0; i < dotCount; i++)
                    {
                        dots[i] = Instantiate(dotPrefab);
                        dots[i].transform.parent = transform;
                    }
                }

                for (int i = 0; i < dotCount; i++)
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
    }

    void ClearDots()
    {
        for (int i = 0; i < dots.Length; i++)
        {
            Destroy(dots[i]);
        }
    }

    public void SetDensity(int density)
    {
        this.density = density;
    }

    public void SetRadius(float radius)
    {
        this.radius = radius;
    }
}
