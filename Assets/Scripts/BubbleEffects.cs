using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleEffects : MonoBehaviour
{
    [SerializeField] private Animator bubbleAnimator;
    [SerializeField] private SpriteRenderer bubbleSprite;
    [SerializeField] private ParticleSystem bubbleParticleSystem;

    public void InBubbleEffect() {
        bubbleAnimator.Play("Default");
        if (!bubbleParticleSystem.isPlaying)
        {
            bubbleParticleSystem.Play();
        }
    }

    public void PopBubbleEffect()
    {
        bubbleAnimator.Play("BubblePop");
        if (bubbleParticleSystem.isPlaying)
        {
            bubbleParticleSystem.Stop();
        }
    }

}
