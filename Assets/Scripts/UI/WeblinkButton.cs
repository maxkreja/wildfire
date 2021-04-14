using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class WeblinkButton : MonoBehaviour
{
    [SerializeField]
    private string href;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        Application.OpenURL(href);
    }
}
