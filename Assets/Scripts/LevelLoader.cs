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
        transition.Play("WipeOut");
    }

    public void PlayNextLevel()
    {
        transition.Play("WipeIn");
    }

}
