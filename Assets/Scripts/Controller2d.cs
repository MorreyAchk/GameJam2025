using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller2d : MonoBehaviour
{
    private bool isJumping;
    private float horizontal;
    private float speed = 8f;
    private bool isFacingRight = true;
    private Rigidbody2D rb;
    private float defaultGravityScale;
    public float bubbleDirectionX;

    public float groundColliderRadius = 0.2f;
    public SpriteRenderer spriteRenderer;
    public bool isInBubble, isFreeze, isMoving;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpingPower = 3f;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private BubbleEffects bubbleEffects;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
    }

    void Update()
    {
        PlayerInput();
        Jumping();
        Animations();
    }

    private void FixedUpdate()
    {
        Vector2 velocity;
        if (isInBubble)
        {
            bubbleEffects.InBubbleEffect();
            rb.gravityScale = 0f;
            velocity = new Vector2(bubbleDirectionX, 2f);
        }
        else
        {
            bubbleEffects.PopBubbleEffect();
            bubbleDirectionX = 0;
            rb.gravityScale = defaultGravityScale;
            velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
        rb.velocity = velocity;
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

    private void PlayerInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        isJumping = Input.GetButtonDown("Jump");
        isMoving = horizontal != 0 || isJumping;
    }

    private void Animations()
    {
        playerAnimator.SetBool("isMoving", isMoving);
        Flip();
    }

    private void Jumping()
    {
        if (isJumping)
        {
            if (IsGrounded())
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
            else if (rb.velocity.y > 0f)
            {
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            spriteRenderer.flipX = !isFacingRight;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag.ToLower().Contains("wall"))
        {
            isInBubble = false;
        }
        else
        {
            Collider2D collider2D = collision.collider;
            BulletTrigger bullet = collider2D.GetComponent<BulletTrigger>();
            if (bullet != null)
            {
                if (bullet.power == Powers.Bubble)
                    isInBubble = true;

                if (bullet.power == Powers.Wind)
                    bubbleDirectionX = bullet.GetComponent<Rigidbody2D>().velocity.x;
            }
        }
    }

    //public void OnSpike()
    //{
    //    if (!bubble.isFreeze)
    //        bubble.isInBubble = false;
    //}

    //public void OnKnowledge(string skill)
    //{
    //    if (skill == "Freeze")
    //    {
    //        bubble.isFreeze = true;
    //    }
    //}
}
