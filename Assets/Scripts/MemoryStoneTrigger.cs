using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryStoneTrigger : MonoBehaviour
{
    public Powers power;
    public Color color;
    public float defaultIntensity = 3f;
    public float intensity=10f;
    public SpriteRenderer innerSprite;
    public float xWindValueForce=10f;
    public new ParticleSystem particleSystem;

    public void Awake()
    {
        Set(power, color, defaultIntensity);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bullet"))
        {
            BulletTrigger bullet = collision.GetComponent<BulletTrigger>();
            Set(bullet.power, bullet.color, intensity);
            particleSystem.GetComponent<ParticleSystemRenderer>().material = innerSprite.material;
            particleSystem.Play();
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Player"))
        {
            Controller2d controller2D = collision.GetComponent<Controller2d>();
            if (power == Powers.Bubble)
            {
                controller2D.UpdateBubbleStateServerRpc(true);
            }
            if (power == Powers.Wind)
            {
                controller2D.bubbleDirectionX = xWindValueForce;
            }

            color = Color.white;
            Set(Powers.Empty, color, defaultIntensity);
        }
    }

    public void Set(Powers power, Color color,float intensity)
    {
        this.power = power;
        innerSprite.material.SetColor("_GlowColor", color * intensity);
    }
}
