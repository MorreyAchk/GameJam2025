using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleInteractable : MonoBehaviour
{
    public bool isInBubble, isFreeze;
    private Rigidbody2D rb;
    private float defaultGravityScale;
    [SerializeField] private float windForce = 20f;
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer bubbleSprite;
    private Vector2 bubbleDirection;
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
            rb.velocity = bubbleDirection + new Vector2(0, 2f);
            rb.gravityScale = 0f;
        }
        else
        {
            GlobalBehaviour.Instance.audioSourceBubble.Stop();
            animator.Play("BubblePop");
            rb.gravityScale = defaultGravityScale;
        }
    }

    public void MoveBubble(Vector3 collisionPosition)
    {
        Vector2 direction = (collisionPosition - transform.position).normalized;
        bubbleDirection = direction * rb.velocity.magnitude * windForce;
        print(bubbleDirection);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isInBubble = false;
    }

}
