using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleInteractable : MonoBehaviour
{
    public bool isInBubble, isFreeze;
    private Rigidbody2D rb;
    private float defaultGravityScale;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer bubbleSprite;
    private float bubbleDirectionX;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
    }
    private void Update()
    {
        if (isInBubble)
        {
            GlobalBehaviour.Instance.audioSourceBubble.Play();
            animator.Play("Default");
            rb.velocity = new Vector2(bubbleDirectionX, 2f);
            rb.gravityScale = 0f;
        }
        else
        {
            bubbleDirectionX = 0;
            GlobalBehaviour.Instance.audioSourceBubble.Stop();
            animator.Play("BubblePop");
            rb.gravityScale = defaultGravityScale;
        }
    }

    public void MoveBubble(float velocityX)
    {
        bubbleDirectionX = velocityX * 2f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isInBubble = false;
    }

}
