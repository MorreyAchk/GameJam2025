using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Controller2d : NetworkBehaviour
{
    private float horizontal, vertical;
    private bool isFacingRight = true;
    private Rigidbody2D rb;


    public float groundColliderRadius = 0.2f;
    public SpriteRenderer playerSprite;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float jumpingPower = 3f;
    [SerializeField] private float speed = 8f;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerMenu playerMenu;
    public ParticleSystem deathParticleSystem;
    private Vector3 networkPosition,previousPosition;
    public InteractFlags currentLever;

    public readonly NetworkVariable<bool> sentToServer = new(true);
    private readonly NetworkVariable<bool> isOnLadder = new(false);
    private readonly NetworkVariable<bool> isFacingRightNetwork = new(true);
    private readonly NetworkVariable<bool> isMoving = new(false,NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    private readonly NetworkVariable<bool> isJumping = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private BulletEffects bulletEffects;
    public AudioSource jumpingAudioSource;
    private AudioSource movingAudioSource;
    public AudioClip walkingClip;
    public AudioClip ladderClip;
    private bool isGrounded;

    private void Start()
    {
        playerMenu.enabled = IsOwner;
        rb = GetComponent<Rigidbody2D>();
        movingAudioSource = GetComponent<AudioSource>();
        bulletEffects = GetComponent<BulletEffects>();
        isFacingRightNetwork.OnValueChanged += OnFacingDirectionChanged;
        isJumping.OnValueChanged += OnJumpingChanged;
        if (IsOwner)
        {
            previousPosition = transform.position;
            SentPositionToServerRpc(previousPosition);
        }
    }

    public override void OnDestroy()
    {
        isFacingRightNetwork.OnValueChanged -= OnFacingDirectionChanged;
        isJumping.OnValueChanged -= OnJumpingChanged;
    }

    void Update()
    {
        IsGrounded();
        MovementSound();
        if (IsOwner)
        {
            InteractWithLever();
            PlayerInput();
            Jumping();
            Animations();
            if (!sentToServer.Value)
                return;
            if (Vector3.Distance(transform.position, previousPosition) > 0)
            {
                previousPosition = transform.position;
                SentPositionToServerRpc(previousPosition);
            }
        }
        else
        {
            transform.position = networkPosition;
        }
    }

    private void MovementSound() {
        movingAudioSource.volume = PlayerPrefs.GetFloat("vfx");
        movingAudioSource.clip = isOnLadder.Value ? ladderClip : walkingClip;
        movingAudioSource.enabled = (isMoving.Value && isGrounded) || (vertical != 0 && isOnLadder.Value);
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

        Vector2 velocity;
        if (bulletEffects.isInBubble.Value)
            return;

        if (isOnLadder.Value)
        {
            velocity = new Vector2(horizontal * speed, vertical * speed);
        }
        else
        {
            velocity = new Vector2(horizontal * speed, rb.velocity.y);
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
        if (bulletEffects.wasInBubble)
            return;

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        isJumping.Value = Input.GetButtonDown("Jump");

        isMoving.Value = horizontal != 0 || isJumping.Value;
    }

    private void Animations()
    {
        playerAnimator.SetBool("isMoving", isMoving.Value);
        Flip();
    }

    private void Jumping()
    {
        if (isJumping.Value)
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

    private void OnJumpingChanged(bool oldValue, bool newValue) {
        if (newValue)
            jumpingAudioSource.PlayOneShot(jumpingAudioSource.clip);
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
        playerSprite.flipX = !newValue;
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
    private void InteractWithLever()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentLever != null)
        {
            currentLever.Interact();
        }
    }

    public void OnLadder()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        isOnLadder.Value = true;
    }

    public void OffLadder()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        isOnLadder.Value = false;
    }

    public void Die()
    {
        if (IsServer)
        {
            ParticleSystem deathParticles = Instantiate(deathParticleSystem, transform.position, Quaternion.identity);
            deathParticles.GetComponent<NetworkObject>().Spawn(true);
            foreach (var player in FindObjectsOfType<Controller2d>(default))
            {
                player.sentToServer.Value = false;
            }
            HideServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void HideServerRpc()
    {
        HideClientRpc();
    }

    [ClientRpc]
    private void HideClientRpc()
    {
        gameObject.SetActive(false);
        GlobalBehaviour.Instance.ResetLoadOutLevelLevel();
    }
}
