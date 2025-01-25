using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject options;

    public void Awake()
    {
    }

    public void OnStart()
    {
      SceneManager.LoadScene("Level1");
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
      GlobalBehaviour.Instance.audioSource.volume = slider.value;
    }

    public void OnBrightnessChange(Slider slider)
    {

    }
}
