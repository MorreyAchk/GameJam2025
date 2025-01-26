using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryStoneTrigger : MonoBehaviour
{
    public Powers power;
    public Color color = Color.white;
    public float xWindValueForce=10f;
    private SpriteRenderer sr;

    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        Set(power, color);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            BulletTrigger bullet = collision.GetComponent<BulletTrigger>();
            Set(bullet.power, bullet.color);
        }

        if (collision.CompareTag("Player"))
        {
            Controller2d controller2D = collision.GetComponent<Controller2d>();
            if (power == Powers.Bubble)
            {
                controller2D.isInBubble = true;
            }
            if (power == Powers.Wind)
            {
                controller2D.bubbleDirectionX = xWindValueForce;
            }

            color = Color.white;
            Set(Powers.Empty, color);
        }
    }

    public void Set(Powers power, Color color)
    {
        this.power = power;
        sr.color = this.color = color;
    }
}
