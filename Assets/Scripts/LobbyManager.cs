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
        inputField.text = "127.0.0.1";
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnected;
        }
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
        playersConnected--;
        NetworkManager.Singleton.Shutdown();
    }

    public void JoinGame()
    {
        StartCoroutine(GlobalBehaviour.Instance.LoadOutLevel(null));
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.ConnectionData.Address = inputField.text;

        if (NetworkManager.Singleton.StartClient())
        {
            Debug.Log("Client started successfully and connecting to server...");
        }
        else
        {
            Debug.LogError("Failed to start client.");
        }
    }

    public void GoBackToMainMenu() => GlobalBehaviour.Instance.BackToMainMenu();

    private void OnClientConnected(ulong clientId)
    {
        playersConnected++;
        if (playersConnected == maxPlayers)
        {
            Debug.Log("All players connected. Starting game...");
            StartGameForAllPlayers();
        }
    }

    private void OnClientDisconnected(ulong clientId)
    {
        playersConnected--;
        GlobalBehaviour.Instance.BackToMainMenu();
        Debug.Log($"Player disconnected. Total players: {playersConnected}");
    }

    private void StartGameForAllPlayers()
    {
        StartCoroutine(GlobalBehaviour.Instance.LoadOutLevel(() => { NetworkManager.Singleton.SceneManager.LoadScene(nextScene, LoadSceneMode.Single); }));
    }
}
