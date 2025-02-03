using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class GlobalBehaviour : MonoBehaviour
{
    private static GlobalBehaviour instance;
    public AudioSource audioSource;
    public AudioClip[] lever, door;
    public Image brightness;

    public GameObject resetShow;
    private float resetTime;

    public static GlobalBehaviour Instance => instance;

    [Header("Level loader")]
    public Animator transition;
    public Image transitionImage;


    public void LoadInLevel()
    {
        transition.Play("WipeOut");
    }

    public IEnumerator LoadOutLevel(Action loadAction)
    {
        transition.Play("WipeIn");
        yield return new WaitForSeconds(transition.GetCurrentAnimatorStateInfo(0).length);
        loadAction.Invoke();
    }

    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Update()
    {
      if (Input.GetKey(KeyCode.X))
      {
        resetShow.SetActive(true);
        resetTime += Time.deltaTime;
        if (resetTime >= 5)
          SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
      }
      else
      {
        resetShow.SetActive(false);
        resetTime = 0;
      }
    }

}
