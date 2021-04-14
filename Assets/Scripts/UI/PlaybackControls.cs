using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlaybackControls : MonoBehaviour
{
    [SerializeField]
    private Button buttonPause;
    [SerializeField]
    private Button buttonPlay;
    [SerializeField]
    private Button buttonSpeedUp;
    [SerializeField]
    private Button buttonQuit;
    [SerializeField]
    private Button buttonQuitConfirm;
    [SerializeField]
    private Button buttonQuitCancel;

    [SerializeField]
    private Color colorActive;
    [SerializeField]
    private Color colorInactive;

    [SerializeField]
    private GameObject panelLoading;
    [SerializeField]
    private GameObject panelQuitConfirm;

    private Image imagePause;
    private Image imagePlay;
    private Image imageSpeedUp;

    void Start()
    {
        imagePause = buttonPause.gameObject.GetComponent<Image>();
        imagePlay = buttonPlay.gameObject.GetComponent<Image>();
        imageSpeedUp = buttonSpeedUp.gameObject.GetComponent<Image>();

        buttonPause.onClick.AddListener(Pause);
        buttonPlay.onClick.AddListener(Play);
        buttonSpeedUp.onClick.AddListener(SpeedUp);
        buttonQuit.onClick.AddListener(OnQuit);
        buttonQuitConfirm.onClick.AddListener(OnQuitConfirm);
        buttonQuitCancel.onClick.AddListener(OnQuitCancel);

        Play();
    }

    private void Pause()
    {
        imagePause.color = colorActive;
        imagePlay.color = colorInactive;
        imageSpeedUp.color = colorInactive;

        Time.timeScale = 0f;
    }

    private void Play()
    {
        imagePause.color = colorInactive;
        imagePlay.color = colorActive;
        imageSpeedUp.color = colorInactive;

        Time.timeScale = 1f;
    }

    private void SpeedUp()
    {
        imagePause.color = colorInactive;
        imagePlay.color = colorInactive;
        imageSpeedUp.color = colorActive;

        Time.timeScale = 2f;
    }

    private void OnQuit()
    {
        Pause();
        panelQuitConfirm.SetActive(true);
    }

    private void OnQuitConfirm()
    {
        panelLoading.SetActive(true);
        SceneManager.LoadScene(0);
    }

    private void OnQuitCancel()
    {
        Play();
        panelQuitConfirm.SetActive(false);
    }
}
