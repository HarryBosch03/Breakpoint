using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Cinemachine;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public class Health : SimpleHealth
{
    [Space]
    [Header("HEALTH")]
    [SerializeField][Range(0.0f, 1.0f)] float maxDamagePercent = 1.0f;

    [Space]
    [SerializeField] protected DamageZone[] damageZones;

    [Space]
    [SerializeField] int buffer = 0;
    [SerializeField] int maxBuffer = 0;

    [Space]
    [SerializeField] float armor = 0.0f;

    [Space]
    [SerializeField] float hardDamageThreshold = 50.0f;
    [SerializeField] float hardDamageMultiplier = 0.25f;
    [SerializeField] float hardDamage = 0.0f;
    [SerializeField] float hardDamageMax = 0.9f;

    [Space]
    [SerializeField] float regenDelay = 0.0f;
    [SerializeField] float regenRate = 0.0f;
    [SerializeField] float regenAcceleration = 0.0f;

    public event System.Action RegenEvent;

    public float HealthCeil => MaxHealth - hardDamage * MaxHealth;

    private void Update()
    {
        if (Time.time > LastDamageTime + regenDelay && CurrentHealth < HealthCeil)
        {
            if (CurrentHealth < 0.0f) CurrentHealth = 0.0f;
            CurrentHealth += regenRate * Time.deltaTime;
            CurrentHealth += regenAcceleration * (Time.time - (LastDamageTime + regenDelay)) * Time.deltaTime;
            if (CurrentHealth > HealthCeil) CurrentHealth = HealthCeil;
            RegenEvent?.Invoke();
        }
    }

    public override void Damage(DamageArgs args)
    {
        if (buffer > 0)
        {
            buffer--;
            args.damage = 0.0f;
        }

        args.damage = Mathf.Max(args.damage - armor / 2.0f, 1.0f);

        args.damage = Mathf.Min(args.damage, MaxHealth * maxDamagePercent);

        foreach (var zone in damageZones)
        {
            zone.Process(ref args);
        }

        if (args.damage > hardDamageThreshold)
        {
            hardDamage += args.damage * hardDamageMultiplier / MaxHealth;
            if (hardDamage > hardDamageMax) hardDamage = hardDamageMax;
        }

        base.Damage(args);
    }

    [System.Serializable]
    public class DamageZone
    {
        public Bounds bounds;
        public Transform parent;
        public float damageScaling;

        public void Process (ref DamageArgs args)
        {
            Vector3 localPoint = parent.InverseTransformPoint(args.point);
            if (bounds.Contains(localPoint))
            {
                args.damage *= damageScaling;
            }
        }

        public void DrawGizmos()
        {
            float t = (1.0f / (-damageScaling - 1.0f)) + 1.0f;
            Gizmos.color = Color.Lerp(Color.green, Color.red, t);
            if (parent) Gizmos.matrix = parent.localToWorldMatrix;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
            Gizmos.color *= new Color(1.0f, 1.0f, 1.0f, 0.2f);
            Gizmos.DrawCube(bounds.center, bounds.size);
        }
    }
}
