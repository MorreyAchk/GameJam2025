using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletEffects : NetworkBehaviour
{
    private Rigidbody2D rb;
    private float defaultGravityScale;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem bubbleParticleSystem;
    public readonly NetworkVariable<Vector2> windForceDirection = new(Vector2.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public readonly NetworkVariable<bool> isInBubble = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public bool wasInBubble;
    public float bubbleUpSpeed = 1f, windImpactForce = 5f;

    [SerializeField] private AudioSource bubbles;
    [SerializeField] private AudioSource bubbleActions;

    [SerializeField] private AudioClip bubblePop;
    [SerializeField] private AudioClip windBlow;

    private void Start()
    {   
        spriteRenderer = bubbles.GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        isInBubble.OnValueChanged += OnBubbleStateChanged;
        windForceDirection.OnValueChanged += OnPushedByWindChanged;
        defaultGravityScale = rb.gravityScale;
    }

    public override void OnDestroy()
    {
        isInBubble.OnValueChanged -= OnBubbleStateChanged;
    }

    private void Update()
    {
        spriteRenderer.enabled = isInBubble.Value;
        if (isInBubble.Value && windForceDirection.Value.Equals(Vector2.zero))
        {
            wasInBubble = true;
            rb.velocity = new Vector2(0f, bubbleUpSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer)
            return;

        if (collision.collider.tag.ToLower().Contains("wall"))
        {
            windForceDirection.Value = Vector3.zero;
            UpdateBubbleState(false);
        }

        if (collision.collider.CompareTag("Bullet"))
        {
            BulletTrigger bullet = collision.collider.GetComponent<BulletTrigger>();
            if (bullet.power == Powers.Bubble)
                UpdateBubbleState(true);

            if (bullet.power == Powers.Wind)
            {
                PushBubble(bullet.direction);
            }

            bullet.DespawnBullet();
        }
    }

    private void OnBubbleStateChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            rb.gravityScale = 0f;
            InBubbleEffect();
        }
        else
        {
            rb.gravityScale = defaultGravityScale;
            PopBubbleEffect();
        }
    }

    private void OnPushedByWindChanged(Vector2 previousValue, Vector2 newValue)
    {
        if (newValue != Vector2.zero)
        {
            rb.AddForce(newValue, ForceMode2D.Impulse);
            bubbleActions.PlayOneShot(windBlow);
        }
    }


    public void UpdateBubbleState(bool newState)
    {
        if (IsServer)
        {
            windForceDirection.Value = Vector2.zero;
            isInBubble.Value = newState;
        }
    }

    public void PushBubble(Vector2 pushDirection)
    {
        windForceDirection.Value = pushDirection;
    }

    private void InBubbleEffect()
    {
        animator.Play("BubbleIn");
        if (!bubbleParticleSystem.isPlaying)
        {
            bubbleParticleSystem.Play();
            bubbles.Play();
        }
    }

    private void PopBubbleEffect()
    {
        animator.Play("BubblePop");
        if (bubbleParticleSystem.isPlaying)
        {
            bubbleParticleSystem.Stop();
            bubbles.Stop();
            bubbleActions.PlayOneShot(bubblePop);
        }
    }

}
