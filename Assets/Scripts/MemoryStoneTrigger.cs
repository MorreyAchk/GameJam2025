using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MemoryStoneTrigger : NetworkBehaviour
{

    public float defaultIntensity = 3f;
    public float intensity = 10f;
    public SpriteRenderer innerSprite;
    public Vector2 windDirection = new (-10f, 0);
    public new ParticleSystem particleSystem;
    public NetworkVariable<MemoryStoneData> memoryStoneData = new(new MemoryStoneData(Powers.Empty, Color.white), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private MemoryStoneData localData = new(Powers.Empty, Color.white);
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        memoryStoneData.OnValueChanged += OnMemoryStoneDataChanged;
    }

    public override void OnDestroy()
    {
        memoryStoneData.OnValueChanged -= OnMemoryStoneDataChanged;
    }
    private void OnMemoryStoneDataChanged(MemoryStoneData previousValue, MemoryStoneData newValue)
    {
        float finalIntensity = Powers.Empty == newValue.power ? defaultIntensity : intensity;
        innerSprite.material.SetColor("_GlowColor", newValue.color * finalIntensity);

        if (Powers.Empty != newValue.power) {
            particleSystem.GetComponent<ParticleSystemRenderer>().material = innerSprite.material;
            particleSystem.Play();
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer)
            return;

        if (collision.CompareTag("Bullet"))
        {
            BulletTrigger bullet = collision.GetComponent<BulletTrigger>();
            SetNetworkData(Powers.Empty, Color.white);
            SetNetworkData(bullet.power, bullet.color);
            bullet.DespawnBullet();
        }
        else if (collision.TryGetComponent<BulletEffects>(out var bulletEffects))
        {
            if (memoryStoneData.Value.power == Powers.Bubble)
            {
                bulletEffects.UpdateBubbleState(true);
            }
            if (memoryStoneData.Value.power == Powers.Wind)
            {
                bulletEffects.PushBubble(windDirection);
            }

            SetNetworkData(Powers.Empty, Color.white);
        }

    }


    private void SetNetworkData(Powers power,Color color)
    {
        localData.power = power;
        localData.color = color;
        memoryStoneData.Value = localData;
    }
}
