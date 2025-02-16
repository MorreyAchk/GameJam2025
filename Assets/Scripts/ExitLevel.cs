using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel : NetworkBehaviour
{
    public string nextScene;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(GlobalBehaviour.Instance.LoadOutLevel(() =>
            {
                if(IsServer)
                    GlobalBehaviour.Instance.LoadLevel(nextScene);
            }));
        }
    }
}
