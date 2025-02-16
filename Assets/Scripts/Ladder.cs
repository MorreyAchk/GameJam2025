using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Ladder : NetworkBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer)
            return;
        if (collision.CompareTag("Player")) {
            collision.GetComponent<Controller2d>().OnLadder();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!IsServer)
            return;
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Controller2d>().OffLadder();
        }
    }
}
