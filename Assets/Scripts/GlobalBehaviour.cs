using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using Unity.Netcode;
using System.Threading.Tasks;

public class GlobalBehaviour : MonoBehaviour
{
    private static GlobalBehaviour instance;
    public AudioSource audioSource;
    public Image brightness;

    public GameObject networkManager;
    public static GlobalBehaviour Instance => instance;

    [Header("Level loader")]
    public Animator transition;
    public Image transitionImage;
    private void Start()
    {
        audioSource.volume = PlayerPrefs.GetFloat("volume");
        StartCoroutine(LoadInLevel());
        if (networkManager != null)
            Instantiate(networkManager);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => StartCoroutine(LoadInLevel());

    private IEnumerator LoadInLevel()
    {
        yield return new WaitForSeconds(1f);
        transition.Play("WipeOut");
    }

    public IEnumerator LoadOutLevel(Action loadAction)
    {
        transition.Play("WipeIn");
        yield return new WaitForSeconds(transition.GetCurrentAnimatorStateInfo(0).length);
        loadAction?.Invoke();
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

    public void LoadLevel(string nextSceneName)
    {
        NetworkManager.Singleton.SceneManager.LoadScene(nextSceneName, LoadSceneMode.Single);
    }
    public void ResetLoadOutLevelLevel()
    {
        StartCoroutine(LoadOutLevel(ResetLevel));
    }

    private void ResetLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;

        if (NetworkManager.Singleton.IsServer)
        {
            LoadLevel(currentSceneName);
        }
        else
            transition.Play("WipeIn");
    }

    public void BackToMainMenu() {
        StartCoroutine(Instance.LoadOutLevel(() =>
        {
            SceneManager.LoadScene(0);
            NetworkManager.Singleton.Shutdown();
        }));
    }

}
