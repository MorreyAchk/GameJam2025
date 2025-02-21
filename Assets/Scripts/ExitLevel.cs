using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel : NetworkBehaviour
{
    public string nextScene;
    private int playerCounter;
    public Controller2d[] players;
    private PlayerSpawner playerSpawner;

    private void Start()
    {
        playerSpawner = FindObjectOfType<PlayerSpawner>();
        players = FindObjectsByType<Controller2d>(default);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerCounter++;
        }

        if (playerCounter == 2)
        {
            if (IsServer)
            {
                if (nextScene == "Credits")
                    playerSpawner.GetComponent<NetworkObject>().Despawn();
                foreach (var player in players)
                {
                    player.sentToServer.Value = false;
                }
            }


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
