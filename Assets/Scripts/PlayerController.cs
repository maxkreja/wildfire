using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(PostProcessVolume))]
public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private LayerData layerData;
    [SerializeField]
    private TextureData textureData;

    [SerializeField]
    private float moveSpeed = 5.0f;
    [SerializeField]
    private float boostSpeed = 10.0f;

    [SerializeField]
    private KeyCode boostKey = KeyCode.LeftShift;

    [SerializeField]
    private float sensivity = 4.0f;

    private float speed;
    private float vertical;
    private float horizontal;

    [SerializeField]
    private Brush brush;
    [SerializeField]
    private ItemBar itemBar;
    private KeyCode[] numericKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9 };

    [SerializeField]
    private TerrainMesh terrainMesh;

    [SerializeField]
    private GameObject panelLoading;

    private AudioSource audioSource;
    private float prevTimeScale;

    void Start()
    {
        itemBar.InitBar();
        terrainMesh.Generate();

        audioSource = GetComponent<AudioSource>();

        panelLoading.SetActive(false);
    }

    void Update()
    {
        bool hoveringUI = EventSystem.current.IsPointerOverGameObject();

        vertical = Input.GetAxisRaw("Vertical") * speed * Time.unscaledDeltaTime;
        horizontal = Input.GetAxisRaw("Horizontal") * speed * Time.unscaledDeltaTime;

        transform.Translate(horizontal, 0, vertical);
        // Cursor.visible = uiHover;

        if (Input.mouseScrollDelta.y > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                brush.DecreaseDensity();
            }
            else
            {
                brush.DecreaseRadius();
            }
        }
        else if (Input.mouseScrollDelta.y < 0)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                brush.IncreaseDensity();
            }
            else
            {
                brush.IncreaseRadius();
            }
        }

        for (int i = 0; i < brush.GetPlantCount(); i++)
        {
            if (Input.GetKeyDown(numericKeys[i]))
            {
                brush.SelectPlant(i);
                itemBar.UpdateBar();
            }
        }

        if (Input.GetMouseButton(1))
        {
            brush.SetBrushEnabled(false);
            Cursor.lockState = CursorLockMode.Locked;

            Vector2 mouseDelta = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            mouseDelta = Vector2.Scale(mouseDelta, new Vector2(sensivity, sensivity));

            transform.Rotate(Vector3.right, -mouseDelta.y);
            transform.Rotate(Vector3.up, mouseDelta.x, Space.World);

            if (Input.GetKey(boostKey))
            {
                speed = boostSpeed;
            }
            else speed = moveSpeed;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;

            if (brush != null)
            {
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerData.terrain.value) && !hoveringUI)
                {
                    brush.UpdatePosition(hit.point, hit.normal);

                    bool foliageAllowed = textureData.FoliageAllowed(hit.point);
                    brush.SetBrushEnabled(foliageAllowed);

                    if (Input.GetMouseButton(0) && !hoveringUI && foliageAllowed)
                    {
                        brush.Paint();
                    }
                }
            }
        }

        updateAudioSource();
    }

    private void updateAudioSource()
    {
        float timeScale = Time.timeScale;
        if (timeScale != prevTimeScale)
        {
            prevTimeScale = timeScale;
            if (timeScale == 0)
            {
                audioSource.Pause();
            }
            else
            {
                audioSource.UnPause();
                audioSource.pitch = timeScale;
            }
        }
    }
}
