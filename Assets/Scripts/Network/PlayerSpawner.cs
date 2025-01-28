using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public bool isInDevelopment;
    public GameObject hostPlayerPrefab;

    private void Awake()
    {
        if (isInDevelopment)
        {
            NetworkManager.Singleton.StartHost();
            SpawnHostPlayer(1u);
        }
    }
    private void Start()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        SpawnAllPlayers();
    }

    private void SpawnAllPlayers()
    {
        foreach (var clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayer(clientId);
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        Debug.Log($"Spawning player for client {clientId}");

        var playerPrefab = NetworkManager.Singleton.NetworkConfig.PlayerPrefab;
        if (playerPrefab != null)
        {
            Vector2 spawnPosition = GetSpawnPosition(clientId);

            var playerInstance = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
            playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
        }
        else
        {
            Debug.LogError("Player prefab not set in NetworkManager.");
        }
    }

    private void SpawnHostPlayer(ulong clientId)
    {
        Vector2 spawnPosition = GetSpawnPosition(clientId);

        var playerInstance = Instantiate(hostPlayerPrefab, spawnPosition, Quaternion.identity);
        playerInstance.GetComponent<NetworkObject>().Spawn();
    }

    private Vector2 GetSpawnPosition(ulong clientId)
    {
        float xOffset = clientId + transform.position.x;
        return new Vector2(xOffset, transform.position.y);
    }

}
