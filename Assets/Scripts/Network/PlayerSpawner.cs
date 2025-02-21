using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
    public bool isInDevelopment;
    public GameObject networkManagerObject;
    public GameObject globalBehaviourObject;

    public GameObject bubblePlayerPrefab;
    public GameObject windPlayerPrefab;

    private void Start()
    {
        if (isInDevelopment && FindFirstObjectByType<NetworkManager>() == null)
        {
            Instantiate(globalBehaviourObject);
            networkManagerObject.SetActive(true);
            NetworkManager.StartHost();
            SpawnAllPlayers();
        }
        else
        {
            if (IsServer)
                NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
        }
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnClientDisconnected(ulong clientId)
    {
        GlobalBehaviour.Instance.BackToMainMenu();
    }

    private void OnIsSceneChanigingChanged(bool previousValue, bool newValue)
    {
        if(newValue)
            GlobalBehaviour.Instance.ResetLoadOutLevelLevel();
    }

    public override void OnDestroy()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.SceneManager != null)
        {
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
    }

    private void OnSceneLoaded(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        SpawnAllPlayers();
    }

    private void SpawnAllPlayers()
    {
        if (!IsServer)
            return;

        var connectedClientIds = NetworkManager.Singleton.ConnectedClientsIds.ToList();
        foreach (var clientId in connectedClientIds)
        {
            SpawnPlayer(clientId);
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        Vector2 spawnPosition = GetSpawnPosition(clientId);
        var playerInstance = Instantiate(clientId == 0 ? bubblePlayerPrefab : windPlayerPrefab, spawnPosition, Quaternion.identity);
        playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId,true);
    }

    private Vector2 GetSpawnPosition(ulong clientId)
    {
        float xOffset = clientId + transform.position.x;
        return new Vector2(xOffset, transform.position.y);
    }

}
