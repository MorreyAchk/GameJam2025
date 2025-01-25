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
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
    }
    private void Update()
    {
        if (isInBubble)
        {
            animator.Play("Default");
            rb.velocity = new Vector2(0, 2f);
            rb.gravityScale = 0f;
        }
        else
        {
            animator.Play("BubblePop");
            rb.gravityScale = defaultGravityScale;
        }
    }

}
