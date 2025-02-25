using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InteractFlags : NetworkBehaviour
{
    public bool isPlate;
    private Animator animator;
    public readonly NetworkVariable<bool> isOn = new(false);
    private AudioSource audioSource;
    private int playerCounter;

    public AudioClip upSound;
    public AudioClip downSound;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponentInChildren<Animator>();
        isOn.OnValueChanged += OnDoorStateChanged;
    }

    public override void OnDestroy()
    {
        isOn.OnValueChanged -= OnDoorStateChanged;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Stone") || collision.CompareTag("Box"))
        {
            if (isPlate && IsServer && collision.GetComponent<NetworkObject>().IsSpawned)
            {
                ToggleFlag(true);
                playerCounter++;
            }
            else if (collision.TryGetComponent<Controller2d>(out var player))
                player.currentLever = this;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Stone"))
        {
            if (isPlate && IsServer && collision.GetComponent<NetworkObject>().IsSpawned)
            {
                playerCounter--;
                if (playerCounter == 0)
                    ToggleFlag(false);
            }
            else if (collision.TryGetComponent<Controller2d>(out var player))
                player.currentLever = null;
        }
    }

    private void OnDoorStateChanged(bool previousValue, bool newValue)
    {
        if(isPlate)
            audioSource.PlayOneShot(newValue? downSound:upSound);
        else
            audioSource.PlayOneShot(audioSource.clip);

        animator.SetBool("ToggleValue", newValue);
    }

    public void Interact()
    {
        if (!isPlate)
        {
            RequestToggleServerRpc();
        }
    }

    private void ToggleFlag(bool newValue)
    {
        if (IsServer)
        {
            isOn.Value = newValue;
        }
        else
        {
            ToggleFlagServerRpc(newValue);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleFlagServerRpc(bool newValue)
    {
        isOn.Value = newValue;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestToggleServerRpc()
    {
        isOn.Value = !isOn.Value;
    }
}
