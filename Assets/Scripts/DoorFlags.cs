using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DoorFlags : NetworkBehaviour
{
    public NetworkVariable<bool> isOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public bool isPlate;
    private Animator animator;

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
        if (collision.CompareTag("Player"))
        {
            if (isPlate)
            {
                isOn.Value = true;
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
        if (collision.CompareTag("Player"))
        {
            if (isPlate)
            {
                isOn.Value = false;
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
        if (!isPlate) // Ensure only non-plates can be interacted with manually
        {
            RequestToggleServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestToggleServerRpc()
    {
        isOn.Value = !isOn.Value;
    }
}
