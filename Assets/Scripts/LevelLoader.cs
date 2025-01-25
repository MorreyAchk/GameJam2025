using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;
    public Image transitionImage;


    void OnEnable()
    {
        SceneManager.sceneLoaded += LoadInLevel;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= LoadInLevel;
    }

    private void LoadInLevel(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(LoadInLevelWithDelay());
    }

    public void PlayNextLevel()
    {
        StartCoroutine(LoadNextLevelWithDelay());
    }

    private IEnumerator LoadInLevelWithDelay()
    {
        transition.Play("WipeOut");
        yield return new WaitForSeconds(0.85f);

        transitionImage.enabled = false;
    }

    private IEnumerator LoadNextLevelWithDelay()
    {
        transitionImage.enabled = true;
        transition.Play("WipeIn");
        yield return new WaitForSeconds(2f);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
