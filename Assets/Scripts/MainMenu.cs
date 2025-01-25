using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject options;
    public LevelLoader levelLoader;

    public void OnStart()
    {
        levelLoader.PlayNextLevel();
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
      Color c = GlobalBehaviour.Instance.brightness.color;
      c.a = 1 - slider.value;
      GlobalBehaviour.Instance.brightness.color = c;
    }
}
