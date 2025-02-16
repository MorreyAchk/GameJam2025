using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletEffects : NetworkBehaviour
{
    private Rigidbody2D rb;
    private float defaultGravityScale,defaultMass;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D parentCollider;
    [SerializeField] private ParticleSystem bubbleParticleSystem;
    public readonly NetworkVariable<bool> isInBubble = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public readonly NetworkVariable<float> bubbleDirectionX = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public bool wasInBubble;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
        isInBubble.OnValueChanged += OnBubbleStateChanged;
        defaultGravityScale = rb.gravityScale;
        defaultMass = rb.mass;
    }

    public override void OnDestroy()
    {
        isInBubble.OnValueChanged -= OnBubbleStateChanged;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer)
            return;

        if (collision.collider.tag.ToLower().Contains("wall"))
        {
            UpdateBubbleState(false);
        }
        if (collision.collider.CompareTag("Bullet"))
        {
            BulletTrigger bullet = collision.collider.GetComponent<BulletTrigger>();
            if (bullet.power == Powers.Bubble)
                UpdateBubbleState(true);

            if (bullet.power == Powers.Wind && isInBubble.Value)
                UpdateBubbleDirection(bullet.GetComponent<Rigidbody2D>().velocity.x);

            bullet.DespawnBullet();
        }
    }

    private void OnBubbleStateChanged(bool oldValue, bool newValue)
    {
        if (newValue)
        {
            rb.gravityScale = 0f;            
            rb.mass = 10f;
            InBubbleEffect();
        }
        else
        {
            if (IsServer)
            {
                bubbleDirectionX.Value = 0;
            }
            rb.gravityScale = defaultGravityScale;
            rb.mass = defaultMass;
            PopBubbleEffect();
        }
    }

    public void UpdateBubbleState(bool newState)
    {
        if (IsServer)
        {
            isInBubble.Value = newState;
        }
    }

    public void UpdateBubbleDirection(float newDirection)
    {
        if (IsServer)
        {
            bubbleDirectionX.Value = newDirection;
        }
    }

    private void InBubbleEffect()
    {
        animator.Play("BubbleIn");
        if (!bubbleParticleSystem.isPlaying)
        {
            bubbleParticleSystem.Play();
        }
    }

    private void PopBubbleEffect()
    {
        animator.Play("BubblePop");
        if (bubbleParticleSystem.isPlaying)
        {
            bubbleParticleSystem.Stop();
        }
    }

}
