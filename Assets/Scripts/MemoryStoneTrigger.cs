using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryStoneTrigger : MonoBehaviour
{
    public Powers power;
    public Color color = Color.white;
    public float xValueForce;

    public void Awake()
    {
        Set(power, color);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BubbleInteractable bubbleInteractable = collision.GetComponentInParent<BubbleInteractable>();
        if (bubbleInteractable != null)
        {
            if (power == Powers.Bubble)
                bubbleInteractable.isInBubble = true;
            if (power == Powers.Wind)
                bubbleInteractable.MoveBubble(xValueForce);

            color = Color.white;
            Set(Powers.Empty, color);
        }
    }

    public void Set(Powers power, Color color)
    {
        this.power = power;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = this.color = color;
    }
}
