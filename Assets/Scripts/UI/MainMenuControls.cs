using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuControls : MonoBehaviour
{
    [SerializeField]
    private Button buttonPlay;
    [SerializeField]
    private Button buttonQuit;
    [SerializeField]
    private Button buttonAbout;
    [SerializeField]
    private Button buttonRandomSeed;
    [SerializeField]
    private Button buttonGenerate;
    [SerializeField]
    private Button buttonCancel;
    [SerializeField]
    private Button buttonBack;

    [SerializeField]
    private InputField inputSeed;

    [SerializeField]
    private GameObject panelSeed;
    [SerializeField]
    private GameObject panelLoading;
    [SerializeField]
    private GameObject panelCredits;

    [SerializeField]
    private MapGeneratorSeed mapGeneratorSeed;

    void Start()
    {
        buttonPlay.onClick.AddListener(OnPlayClick);
        buttonQuit.onClick.AddListener(OnQuitClick);
        buttonAbout.onClick.AddListener(OnAboutClick);
        buttonRandomSeed.onClick.AddListener(OnRandomSeedClick);
        buttonGenerate.onClick.AddListener(OnGenerateClick);
        buttonCancel.onClick.AddListener(OnCancelClick);
        buttonBack.onClick.AddListener(OnBackClick);

        Random.InitState((int)System.DateTime.Now.Ticks);

        OnRandomSeedClick();
    }

    private void OnPlayClick()
    {
        panelSeed.SetActive(true);
    }

    private void OnQuitClick()
    {
        Application.Quit();
    }

    private void OnAboutClick()
    {
        panelCredits.SetActive(true);
    }

    private void OnBackClick()
    {
        panelCredits.SetActive(false);
    }

    private void OnRandomSeedClick()
    {
        string seed = "";
        for (int i = 0; i < 9; i++)
        {
            seed = seed + Random.Range(0, 10);
        }
        inputSeed.text = seed;
    }

    private void OnGenerateClick()
    {
        panelLoading.SetActive(true);
        mapGeneratorSeed.SetSeed(int.Parse(inputSeed.text));
        SceneManager.LoadScene(1);
    }

    private void OnCancelClick()
    {
        panelSeed.SetActive(false);
    }
}
