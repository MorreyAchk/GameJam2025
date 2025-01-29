using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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

    public AudioClip GetLeverClip() => lever[Random.Range(0, lever.Length)];

    public AudioClip GetDoorClip() => door[Random.Range(0, door.Length)];
}
