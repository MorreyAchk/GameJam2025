using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MoveableObject : NetworkBehaviour
{
    private Rigidbody2D rb;
    private BulletEffects bulletEffects;
    public float defaultUpBubbleSpeed;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletEffects = GetComponent<BulletEffects>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
            rb.velocity = Vector3.zero;

        if (!IsServer)
            return;

        if (bulletEffects.wasInBubble && collision.collider.CompareTag("DestroyableWalls"))
        {
            collision.gameObject.GetComponent<NetworkObject>().Despawn(true);
        }
    }
}
