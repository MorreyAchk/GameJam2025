using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Controller2d : NetworkBehaviour
{
    public bool isJumping,isMoving;
    private float horizontal;
    private bool isFacingRight = true;
    private Rigidbody2D rb;
    private float defaultGravityScale;

    public float groundColliderRadius = 0.2f;
    public SpriteRenderer spriteRenderer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpingPower = 3f;
    [SerializeField] private float speed = 8f;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private BubbleEffects bubbleEffects;
    private Vector3 networkPosition;
    public DoorFlags currentLever;

    private readonly NetworkVariable<bool> isFacingRightNetwork = new (true);
    private readonly NetworkVariable<bool> isInBubbleNetwork = new (false);
    private readonly NetworkVariable<float> bubbleDirectionX = new(0f);
    public bool wasInBubble,isGrounded;

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
        if (IsOwner)
        {
            IsGrounded();
            InteractWithLever();
            PlayerInput();
            Jumping();
            Animations();
            SentPositionToServerRpc(transform.position);
        }
        else {
            transform.position = networkPosition;
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

        Vector2 velocity;
        if (isInBubbleNetwork.Value)
        {
            wasInBubble = true;
            rb.gravityScale = 0f;
            velocity = new Vector2(bubbleDirectionX.Value, 2f);
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
            velocity = new Vector2(horizontal * speed, rb.velocity.y);
        }
        rb.velocity = velocity;
    }

    private void IsGrounded()
    {
        isGrounded =  Physics2D.OverlapCircle(groundCheck.position, groundColliderRadius, groundLayer);
        if(isGrounded && wasInBubble)
            wasInBubble = false;
    }

    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundColliderRadius);
        }
    }

    #region PlayerMovement
    private void PlayerInput()
    {
        isMoving = !isInBubbleNetwork.Value && (horizontal != 0 || isJumping);
        if (wasInBubble)
            return;
        horizontal = Input.GetAxisRaw("Horizontal");
        isJumping = Input.GetButtonDown("Jump");
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
            if (isGrounded)
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

    [ServerRpc]
    private void UpdateFacingDirectionServerRpc(bool newFacingRight)
    {
        isFacingRightNetwork.Value = newFacingRight;
    }

    [ServerRpc]
    private void SentPositionToServerRpc(Vector3 position) => SentPositionFromClientRpc(position);

    [ClientRpc]
    private void SentPositionFromClientRpc(Vector3 position)
    {
        if (IsOwner)
            return;

        networkPosition = position;
    }

    #endregion

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer)
            return;

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
                    bubbleDirectionX.Value = bullet.GetComponent<Rigidbody2D>().velocity.x;
                }
            }
            bullet.DespawnBullet();
        }
    }

    private void OnBubbleStateChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            bubbleEffects.InBubbleEffect();
        }
        else
        {
            if (IsServer) {
                bubbleDirectionX.Value = 0;
            }
            bubbleEffects.PopBubbleEffect();
        }
    }

    public void UpdateBubbleState(bool newState)
    {
        if (IsServer)
        {
            isInBubbleNetwork.Value = newState;
        }
    }

    public void UpdateBubbleDirection(float newDirection)
    {
        if (IsServer)
        {
            bubbleDirectionX.Value = newDirection;
        }
    }


    private void InteractWithLever() {
        if (Input.GetKeyDown(KeyCode.E) && currentLever != null)
        {
            currentLever.Interact();
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
