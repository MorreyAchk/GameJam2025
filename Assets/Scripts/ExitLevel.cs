using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel : MonoBehaviour
{
    public string nextScene;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(GlobalBehaviour.Instance.LoadOutLevel(() => { NetworkManager.Singleton.SceneManager.LoadScene(nextScene, LoadSceneMode.Single); }));
        }
    }
}
