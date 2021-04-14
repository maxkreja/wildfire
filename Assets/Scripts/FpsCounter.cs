using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class FpsCounter : MonoBehaviour
{
    private Text text;
    private float deltaTime;

    void Start()
    {
        text = GetComponent<Text>();
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1f / deltaTime;
        text.text = Mathf.RoundToInt(fps).ToString();
    }
}
