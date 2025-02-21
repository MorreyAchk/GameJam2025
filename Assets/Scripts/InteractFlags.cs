using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InteractFlags : NetworkBehaviour
{
    public bool isPlate;
    private Animator animator;
    public readonly NetworkVariable<bool> isOn = new(false);
    private int playerCounter;

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        isOn.OnValueChanged += OnDoorStateChanged;
    }

    public override void OnDestroy()
    {
        isOn.OnValueChanged -= OnDoorStateChanged;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer)
            return;

        if (collision.CompareTag("Player") || collision.CompareTag("Stone") || collision.CompareTag("Box"))
        {
            if (isPlate)
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
        if (!IsServer)
            return;

        if (collision.CompareTag("Player") || collision.CompareTag("Stone"))
        {
            if (isPlate && collision.GetComponent<NetworkObject>().IsSpawned)
            {
                playerCounter--;
                if (playerCounter == 0)
                    ToggleFlag(false);
            }
            else if (collision.TryGetComponent<Controller2d>(out var player))
                player.currentLever = this;
        }
    }

    private void OnDoorStateChanged(bool previousValue, bool newValue)
    {
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
