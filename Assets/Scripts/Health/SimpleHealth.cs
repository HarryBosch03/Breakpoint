using System;
using Unity.Netcode;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public class SimpleHealth : Damagable
{
    [Space]
    [Header("SIMPLE HEALTH")]
    [SerializeField] NetworkVariable<float> normalizedHealth = new NetworkVariable<float>();
    [SerializeField] float maxHealth;
    [SerializeField] bool disableOnDeath;

    public event System.Action<DamageArgs> DeathEvent;
    NetworkVariable<float> lastDamageTime = new NetworkVariable<float>();

    public float LastDamageTime => lastDamageTime.Value;

    public float CurrentHealth
    {
        get => normalizedHealth.Value * maxHealth;
        set => normalizedHealth.Value = Mathf.Min(value / maxHealth, 1.0f);
    }
    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            if (!IsServer) return;
            maxHealth = value;
        }
    }
    public float NormalizedHealth => normalizedHealth.Value;

    private void OnEnable()
    {
        if (!IsServer) return;
        normalizedHealth.Value = 1.0f;
    }

    public override void Damage(DamageArgs args)
    {
        base.Damage(args);

        if (IsServer)
        {
            CurrentHealth -= args.damage;
            lastDamageTime.Value = Time.time;

            if (CurrentHealth < 0.0f)
            {
                Die(args);
            }
        }
    }

    public virtual void Die(DamageArgs args)
    {
        if (!IsServer) return;

        DeathEvent?.Invoke(args);

        if (disableOnDeath)
        {
            gameObject.SetActive(false);
            DieClientRPC();
        }
        else
        {
            NetworkObject.Despawn();
        }
    }

    [ClientRpc]
    private void DieClientRPC()
    {
        if (IsServer) return;

        gameObject.SetActive(false);
    }
}
