using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject options;

    private void Start()
    {
        GlobalBehaviour.Instance.LoadInLevel();
        GlobalBehaviour.Instance.audioSource.volume = PlayerPrefs.GetFloat("volume");
    }

    public void OnStart()
    {
        StartCoroutine(GlobalBehaviour.Instance.LoadOutLevel(() => { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); }));
    }

    public void OnOptions()
    {
        options.SetActive(!options.activeSelf);
    }

    public void OnOptionsBack()
    {
        options.SetActive(!options.activeSelf);
    }

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
        GlobalBehaviour.Instance.audioSource.volume = PlayerPrefs.GetFloat("volume");
    }

    public void OnBrightnessChange(Slider slider)
    {
        Color c = GlobalBehaviour.Instance.brightness.color;
        c.a = 1 - slider.value;
        GlobalBehaviour.Instance.brightness.color = c;
    }
}
