using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Controller2d : NetworkBehaviour
{
    public bool isJumping, isMoving;
    private float horizontal, vertical;
    private bool isFacingRight = true;
    private Rigidbody2D rb;


    public float groundColliderRadius = 0.2f;
    public SpriteRenderer spriteRenderer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpingPower = 3f;
    [SerializeField] private float speed = 8f;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private BulletEffects bubbleEffects;
    private Vector3 networkPosition;
    public InteractFlags currentLever;

    private readonly NetworkVariable<bool> isOnLadder = new(false);
    private readonly NetworkVariable<bool> isFacingRightNetwork = new(true);
    private BulletEffects bulletEffects;
    private bool isGrounded;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bulletEffects= GetComponent<BulletEffects>();

        isFacingRightNetwork.OnValueChanged += OnFacingDirectionChanged;
    }

    public override void OnDestroy()
    {
        isFacingRightNetwork.OnValueChanged -= OnFacingDirectionChanged;
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
        else
        {
            transform.position = networkPosition;
        }
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

        Vector2 velocity;
        if (bulletEffects.isInBubbleNetwork.Value)
        {
            bulletEffects.wasInBubble = true;
            velocity = new Vector2(bulletEffects.bubbleDirectionX.Value, 2f);
        }
        else
        {
            if (isOnLadder.Value)
            {
                velocity = new Vector2((vertical + horizontal) * speed, rb.velocity.y);
            }
            else
            {
                velocity = new Vector2(horizontal * speed, rb.velocity.y);
            }
        }
        rb.velocity = velocity;
    }

    private void IsGrounded()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundColliderRadius, groundLayer);
        if (isGrounded && bulletEffects.wasInBubble)
            bulletEffects.wasInBubble = false;
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
        isMoving = !bulletEffects.isInBubbleNetwork.Value && (horizontal != 0 || isJumping);
        if (bulletEffects.wasInBubble)
            return;
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
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

        if (collision.collider.CompareTag("Ladder"))
        {
            isOnLadder.Value = true;
        }
    }

    private void InteractWithLever()
    {
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
