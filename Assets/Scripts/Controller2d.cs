using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2d : MonoBehaviour
{
    private bool isJumping;
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 16f;
    private bool isFacingRight = true;

    [SerializeField] private float groundColliderRadius = 0.2f;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private BubbleInteractable bubble;


    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        isJumping = Input.GetButtonDown("Jump");

        if (isJumping && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }

        if (isJumping && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
        if (isJumping)
        {
            bubble.isInBubble = false;
        }

        Flip();
    }

    private void FixedUpdate()
    {
        if (!IsGrounded())
            return;
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundColliderRadius, groundLayer);
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireSphere(groundCheck.position, groundColliderRadius);
        }
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public void SetInBubble()
    {
        bubble.isInBubble = true;
    }
}
