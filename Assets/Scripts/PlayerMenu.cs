using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMenu : MonoBehaviour
{
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject parentObject;
    [SerializeField] private AudioSource VFXTestSound;

    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider vfxSlider;
    [SerializeField] private Slider brightnesSlider;

    private void Start()
    {
        int firstRun = PlayerPrefs.GetInt("savedFirstRun");
        if (firstRun == 0)
        {
            PlayerPrefs.SetFloat("volume", 0.5f);
            PlayerPrefs.SetFloat("vfx", 0.5f);
            PlayerPrefs.SetFloat("brightness", 0.5f);
            PlayerPrefs.SetInt("savedFirstRun", 1);
        }

        volumeSlider.value = PlayerPrefs.GetFloat("volume");
        vfxSlider.value = PlayerPrefs.GetFloat("vfx");
        brightnesSlider.value = PlayerPrefs.GetFloat("brightness");
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            ToggleMenu();
    }

    public void OnStart()
    {
        StartCoroutine(GlobalBehaviour.Instance.LoadOutLevel(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); }));
    }

    public void ToggleMenu()
    {
        parentObject.SetActive(!parentObject.activeSelf);
    }

    public void ToggleOptions()
    {
        options.SetActive(!options.activeSelf);
        mainMenu.SetActive(!mainMenu.activeSelf);
    }

    public void Disconnect() => GlobalBehaviour.Instance.BackToMainMenu();
    public void OnExit()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void OnVolumeChange(Slider slider)
    {
        PlayerPrefs.SetFloat("volume", slider.value);
        GlobalBehaviour.Instance.audioSource.volume = slider.value;
    }

    public void OnVfxChange(Slider slider)
    {
        PlayerPrefs.SetFloat("vfx", slider.value);
        VFXTestSound.volume = slider.value;
        if(!VFXTestSound.isPlaying && options.activeSelf)
            VFXTestSound.Play();
    }

    public void OnBrightnessChange(Slider slider)
    {
        UpdateBrightness(slider.value);
    }

    private void UpdateBrightness(float value) {
        PlayerPrefs.SetFloat("brightness", value);
        Color c = GlobalBehaviour.Instance.brightness.color;
        c.a = 1 - value;
        GlobalBehaviour.Instance.brightness.color = c;
    }
}
