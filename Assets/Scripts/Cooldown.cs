using System;
using UnityEngine;

[Serializable]
public class Cooldown
{
    [SerializeField] private float cooldownTime;
    private float _nextFireTime;

    public bool IsCoolingDown => Time.time < _nextFireTime;

    public void StartCooldown() => _nextFireTime = Time.time + cooldownTime;
}
