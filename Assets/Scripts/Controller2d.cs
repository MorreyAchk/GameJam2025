using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Controller2d : NetworkBehaviour
{
    public bool isJumping,isMoving;
    private float horizontal;
    private float speed = 8f;
    private bool isFacingRight = true;
    private Rigidbody2D rb;
    private float defaultGravityScale;
    public float bubbleDirectionX;

    public float groundColliderRadius = 0.2f;
    public SpriteRenderer spriteRenderer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpingPower = 3f;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private BubbleEffects bubbleEffects;

    private readonly NetworkVariable<bool> isFacingRightNetwork = new (true);
    private readonly NetworkVariable<bool> isInBubbleNetwork = new (false);

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;

        isFacingRightNetwork.OnValueChanged += OnFacingDirectionChanged;
        isInBubbleNetwork.OnValueChanged += OnBubbleStateChanged;
    }

    public override void OnDestroy()
    {
        isFacingRightNetwork.OnValueChanged -= OnFacingDirectionChanged;
        isInBubbleNetwork.OnValueChanged -= OnBubbleStateChanged;
    }

    void Update()
    {
        if (!IsOwner)
            return;

        PlayerInput();
        Jumping();
        Animations();
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

        Vector2 velocity;
        if (isInBubbleNetwork.Value)
        {
            rb.gravityScale = 0f;
            velocity = new Vector2(bubbleDirectionX, 2f);
        }
        else
        {
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
        if ((isFacingRight && horizontal < 0f) || (!isFacingRight && horizontal > 0f))
        {
            isFacingRight = !isFacingRight;

            if (IsServer)
            {
                isFacingRightNetwork.Value = isFacingRight;
            }
            else
            {
                UpdateFacingDirectionServerRpc(isFacingRight);
            }
        }
    }

    private void OnFacingDirectionChanged(bool oldValue, bool newValue)
    {
        spriteRenderer.flipX = !newValue;
    }

    private void OnBubbleStateChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            bubbleEffects.InBubbleEffect();
        }
        else
        {
            bubbleEffects.PopBubbleEffect();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag.ToLower().Contains("wall"))
        {
            UpdateBubbleState(false);
        }
        else
        {
            BulletTrigger bullet = collision.collider.GetComponent<BulletTrigger>();
            if (bullet != null)
            {
                if (bullet.power == Powers.Bubble)
                {
                    UpdateBubbleState(true);
                }

                if (bullet.power == Powers.Wind)
                {
                    bubbleDirectionX = bullet.GetComponent<Rigidbody2D>().velocity.x;
                }
            }
        }
    }

    private void UpdateBubbleState(bool newState)
    {
        if (IsServer)
        {
            isInBubbleNetwork.Value = newState;
        }
        else
        {
            UpdateBubbleStateServerRpc(newState);
        }
    }

    [ServerRpc]
    public void UpdateBubbleStateServerRpc(bool newState)
    {
        isInBubbleNetwork.Value = newState;
    }

    [ServerRpc]
    private void UpdateFacingDirectionServerRpc(bool newFacingRight)
    {
        isFacingRightNetwork.Value = newFacingRight;
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
