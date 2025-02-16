using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BulletEffects : NetworkBehaviour
{
    private Rigidbody2D rb;
    private float defaultGravityScale;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D parentCollider;
    [SerializeField] private ParticleSystem bubbleParticleSystem;
    public readonly NetworkVariable<bool> isInBubble = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    public bool wasInBubble;
    public float bubbleUpSpeed = 1f,windImpactForce=5f;
    private bool pushedByWind = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>(); 
        isInBubble.OnValueChanged += OnBubbleStateChanged;
        defaultGravityScale = rb.gravityScale;
    }

    public override void OnDestroy()
    {
        isInBubble.OnValueChanged -= OnBubbleStateChanged;
    }

    private void Update()
    {
        if (isInBubble.Value && !pushedByWind)
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
            pushedByWind = false;
            UpdateBubbleState(false);
        }
            
        if (collision.collider.CompareTag("Bullet"))
        {
            BulletTrigger bullet = collision.collider.GetComponent<BulletTrigger>();
            if (bullet.power == Powers.Bubble)
                UpdateBubbleState(true);

            if (bullet.power == Powers.Wind)
            {
                pushedByWind = true;
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                PushBubble(pushDirection);
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

    public void UpdateBubbleState(bool newState)
    {
        if (IsServer)
        {
            isInBubble.Value = newState;
        }
    }

    public void PushBubble(Vector2 pushDirection) {
        rb.AddForce(pushDirection * windImpactForce, ForceMode2D.Impulse);
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
