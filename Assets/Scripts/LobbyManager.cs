using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    private int playersConnected = 0;
    public int maxPlayers = 2;
    public TMP_InputField inputField;
    public GameObject options;
    public GameObject waitScreen;
    public string nextScene;

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    public void StartGame()
    {
        options.SetActive(false);
        waitScreen.SetActive(true);
        if (NetworkManager.Singleton.StartHost())
        {
            Debug.Log("Host started successfully. Waiting for players...");
        }
        else
        {
            Debug.LogError("Failed to start host.");
        }
    }

    public void StopGame() {
        waitScreen.SetActive(false);
        options.SetActive(true);
        NetworkManager.Singleton.Shutdown();
    }

    public void JoinGame()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = "127.0.0.1";

        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client started successfully and connecting to server...");
        }
        else
        {
            Debug.LogError("Failed to start client.");
        }
    }

    private void OnClientConnected(ulong clientId)
    {
        playersConnected++;
        Debug.Log($"Player connected. Total players: {playersConnected}");

        if (playersConnected == maxPlayers)
        {
            Debug.Log("All players connected. Starting game...");
            StartGameForAllPlayers();
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        playersConnected--;
        Debug.Log($"Player disconnected. Total players: {playersConnected}");
    }

    private void StartGameForAllPlayers()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(nextScene, LoadSceneMode.Single);
    }
}
