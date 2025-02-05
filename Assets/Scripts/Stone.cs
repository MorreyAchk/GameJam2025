using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Stone : NetworkBehaviour
{
    public bool isInBubble, isFreeze, wasInBubble;
    private Rigidbody2D rb;
    private float bubbleDirectionX;
    private float defaultGravityScale;
    private BubbleEffects bubbleEffects;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bubbleEffects = GetComponent<BubbleEffects>();
        defaultGravityScale = rb.gravityScale;
    }
    private void Update()
    {
        if (isInBubble)
        {
            rb.gravityScale = 0f;
            bubbleEffects.InBubbleEffect();
            rb.velocity = new Vector2(bubbleDirectionX, 2f);
        }
        else
        {
            bubbleDirectionX = 0;
            bubbleEffects.PopBubbleEffect();
            rb.gravityScale = defaultGravityScale;
        }
    }

    public void MoveBubble(float velocityX)
    {
        bubbleDirectionX = velocityX * 2f;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer)
            return;

        if (collision.collider.tag.ToLower().Contains("wall"))
        { 
            isInBubble = false;
        }
        if (collision.collider.CompareTag("Bullet"))
        {
            Collider2D collider2D = collision.collider;
            BulletTrigger bullet = collider2D.GetComponent<BulletTrigger>();
            if (bullet.power == Powers.Bubble)
            {
                wasInBubble = true;
                isInBubble = true;
            }

            if (bullet.power == Powers.Wind)
                bubbleDirectionX = bullet.GetComponent<Rigidbody2D>().velocity.x;

            bullet.DespawnBullet();
        }
        if (wasInBubble && collision.collider.CompareTag("DestroyableWalls"))
        {
            collision.gameObject.GetComponent<NetworkObject>().Despawn(true);
        }
    }
}
