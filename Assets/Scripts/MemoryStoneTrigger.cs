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
    public float xWindValueForce = 10f;
    public new ParticleSystem particleSystem;
    public NetworkVariable<MemoryStoneData> memoryStoneData = new(new MemoryStoneData(Powers.Empty, Color.white), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private MemoryStoneData localData = new(Powers.Empty, Color.white);

    private void Start()
    {
        memoryStoneData.OnValueChanged += OnMemoryStoneDataChanged;
    }

    private void OnMemoryStoneDataChanged(MemoryStoneData previousValue, MemoryStoneData newValue)
    {
        float finalIntensity = Powers.Empty == newValue.power ? defaultIntensity : intensity;
        innerSprite.material.SetColor("_GlowColor", newValue.color * finalIntensity);

        if (Powers.Empty != newValue.power) {
            particleSystem.GetComponent<ParticleSystemRenderer>().material = innerSprite.material;
            particleSystem.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer)
            return;

        if (collision.CompareTag("Bullet"))
        {
            BulletTrigger bullet = collision.GetComponent<BulletTrigger>();
            SetNetworkData(bullet.power, bullet.color);
            bullet.DespawnBullet();
        }

        if (collision.CompareTag("Player"))
        {
            Controller2d controller2D = collision.GetComponent<Controller2d>();
            if (memoryStoneData.Value.power == Powers.Bubble)
            {
                controller2D.UpdateBubbleState(true);
            }
            if (memoryStoneData.Value.power == Powers.Wind)
            {
                controller2D.UpdateBubbleDirection(xWindValueForce);
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
