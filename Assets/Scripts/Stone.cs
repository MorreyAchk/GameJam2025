using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Stone : NetworkBehaviour
{

    private Rigidbody2D rb;
    private BulletEffects bulletEffects;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletEffects = GetComponent<BulletEffects>();
    }
    private void Update()
    {
        if (bulletEffects.isInBubble.Value)
        {
            rb.velocity = new Vector2(bulletEffects.bubbleDirectionX.Value, 2f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer)
            return;

        if (bulletEffects.wasInBubble && collision.collider.CompareTag("DestroyableWalls"))
        {
            collision.gameObject.GetComponent<NetworkObject>().Despawn(true);
        }
    }
}
