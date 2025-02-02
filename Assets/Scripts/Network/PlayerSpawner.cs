using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : MonoBehaviour
{
    public bool isInDevelopment;
    public GameObject bubblePlayerPrefab;
    public GameObject windPlayerPrefab;

    private void Start()
    {
        if (isInDevelopment)
        {
            NetworkManager.Singleton.StartHost();
        }
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

        Vector2 spawnPosition = GetSpawnPosition(clientId);
        
        var playerInstance = Instantiate(clientId == 0 ? bubblePlayerPrefab : windPlayerPrefab, spawnPosition, Quaternion.identity);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId);
    }

    private Vector2 GetSpawnPosition(ulong clientId)
    {
        float xOffset = clientId + transform.position.x;
        return new Vector2(xOffset, transform.position.y);
    }

}
