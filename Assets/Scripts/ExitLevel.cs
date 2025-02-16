using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel : NetworkBehaviour
{
    public string nextScene;
    private PlayerSpawner playerSpawner;
    private int playerCounter;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        playerSpawner = FindObjectOfType<PlayerSpawner>();

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerCounter++;
        }

        if (playerCounter == 2) {
            if (IsServer)
                playerSpawner.isSceneChaniging.Value = true;

            StartCoroutine(GlobalBehaviour.Instance.LoadOutLevel(() =>
            {
                if (IsServer)
                {
                    GlobalBehaviour.Instance.LoadLevel(nextScene);
                }
            }));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerCounter--;
        }
    }
}
