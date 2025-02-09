using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletEffects : NetworkBehaviour
{
    public Rigidbody2D rb;
    private float defaultGravityScale;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem bubbleParticleSystem;
    [SerializeField] private ParticleSystem snowflakeParticleSystem;
    public readonly NetworkVariable<bool> isFrozen = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public readonly NetworkVariable<bool> isInBubbleNetwork = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public readonly NetworkVariable<float> bubbleDirectionX = new(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public bool wasInBubble;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
        isInBubbleNetwork.OnValueChanged += OnBubbleStateChanged;
        isFrozen.OnValueChanged += OnFrozenStateChanged;
        defaultGravityScale = rb.gravityScale;
    }

    public override void OnDestroy()
    {
        isInBubbleNetwork.OnValueChanged -= OnBubbleStateChanged;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer || isFrozen.Value)
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

            if (bullet.power == Powers.Wind && isInBubbleNetwork.Value)
                UpdateBubbleDirection(bullet.GetComponent<Rigidbody2D>().velocity.x);

            if (bullet.power == Powers.Ice)
                UpdateFrozenState(true);

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
            if (IsServer)
            {
                bubbleDirectionX.Value = 0;
            }
            rb.gravityScale = defaultGravityScale;
            PopBubbleEffect();
        }
    }

    private void OnFrozenStateChanged(bool previousValue, bool newValue)
    {
        if (!IsServer)
            return;

        if (newValue)
        {
            isInBubbleNetwork.Value = false;
            bubbleDirectionX.Value = 0;
            FreezeBubbleEffect();
        }
        else
        {
            UnFreezeBubbleEffect();
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

    public void UpdateFrozenState(bool newState)
    {
        if (IsServer)
        {
            isFrozen.Value = newState;
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

    private void FreezeBubbleEffect()
    {
        animator.Play("IceIn");
        if (snowflakeParticleSystem.isPlaying)
        {
            snowflakeParticleSystem.Play();
        }
    }

    private void UnFreezeBubbleEffect()
    {
        animator.Play("IceOut");
        if (snowflakeParticleSystem.isPlaying)
        {
            snowflakeParticleSystem.Stop();
        }
    }

}
