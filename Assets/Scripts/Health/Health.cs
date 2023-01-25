using Unity.Netcode;
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
    [SerializeField] NetworkVariable<int> buffer;
    [SerializeField] int maxBuffer = 0;

    [Space]
    [SerializeField] NetworkVariable<float> armor;

    [Space]
    [SerializeField] float hardDamageThreshold = 50.0f;
    [SerializeField] float hardDamageMultiplier = 0.25f;
    [SerializeField] NetworkVariable<float> hardDamage;
    [SerializeField] float hardDamageMax = 0.9f;

    [Space]
    [SerializeField] float regenDelay = 0.0f;
    [SerializeField] float regenRate = 0.0f;
    [SerializeField] float regenAcceleration = 0.0f;

    public event System.Action RegenEvent;

    public float HealthCeil => MaxHealth - hardDamage.Value * MaxHealth;

    private void Update()
    {
        if (Time.time > LastDamageTime + regenDelay && CurrentHealth < HealthCeil && IsServer)
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
        if (!IsServer)
        {
            base.Damage(args);
            return;
        }

        if (buffer.Value > 0)
        {
            buffer.Value--;
            args.damage = 0.0f;
        }

        args.damage = Mathf.Max(args.damage - armor.Value / 2.0f, 1.0f);

        args.damage = Mathf.Min(args.damage, MaxHealth * maxDamagePercent);

        foreach (var zone in damageZones)
        {
            zone.Process(ref args);
        }

        if (args.damage > hardDamageThreshold)
        {
            hardDamage.Value += args.damage * hardDamageMultiplier / MaxHealth;
            if (hardDamage.Value > hardDamageMax) hardDamage.Value = hardDamageMax;
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
