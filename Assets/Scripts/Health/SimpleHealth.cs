using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public class SimpleHealth : Damagable
{
    [Space]
    [Header("SIMPLE HEALTH")]
    [SerializeField][Range(0.0f, 1.0f)] float normalizedHealth;
    [SerializeField] float maxHealth;
    [SerializeField] bool disableOnDeath;

    public float LastDamageTime { get; set; }

    public float CurrentHealth
    {
        get => normalizedHealth * maxHealth;
        set => normalizedHealth = Mathf.Min(value / maxHealth, 1.0f);
    }
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float NormalizedHealth => normalizedHealth;

    public override void Damage(DamageArgs args)
    {
        LastDamageTime = Time.time;

        CurrentHealth -= args.damage;

        base.Damage(args);

        if (CurrentHealth < 0.0f)
        {
            Die(args);
        }
    }

    public virtual void Die(DamageArgs args)
    {
        if (disableOnDeath) gameObject.SetActive(false);
        else Destroy(gameObject);
    }
}
