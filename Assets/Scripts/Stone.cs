using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
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
        if (collision.collider.CompareTag("Wall")) { 
            isInBubble = false;
        }
        else
        {
            Collider2D collider2D = collision.collider;
            BulletTrigger bullet = collider2D.GetComponent<BulletTrigger>();
            if (bullet != null)
            {
                if (bullet.power == Powers.Bubble)
                {
                    wasInBubble = true;
                    isInBubble = true;
                }

                if (bullet.power == Powers.Wind)
                    bubbleDirectionX = bullet.GetComponent<Rigidbody2D>().velocity.x;
            }
        }
        if (wasInBubble && collision.collider.CompareTag("DestroyableWalls"))
        {
            Destroy(collision.gameObject);
        }
    }
}
