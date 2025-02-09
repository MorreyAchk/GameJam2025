using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class InteractFlags : NetworkBehaviour
{
    public bool isPlate;
    private Animator animator;
    public readonly NetworkVariable<bool> isOn = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


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
        if (collision.CompareTag("Player") || collision.CompareTag("Stone"))
        {
            if (isPlate)
            {
                ToggleFlag(true);
            }
            else
            {
                Controller2d player = collision.GetComponent<Controller2d>();
                player.currentLever = this;
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Stone"))
        {
            if (isPlate)
            {
                ToggleFlag(false);
            }
            else
            {
                Controller2d player = collision.GetComponent<Controller2d>();
                player.currentLever = null;
            }
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
