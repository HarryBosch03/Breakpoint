using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public class Damagable : MonoBehaviour
{
    [Header("DAMAGABLE")]
    public GameObject hitFX;

    public event System.Action<DamageArgs> DamageEvent;

    public virtual void Damage (DamageArgs args)
    {
        if (hitFX) Instantiate(hitFX, args.point, Quaternion.Euler(args.normal));
        DamageEvent?.Invoke(args);
    }
}

public struct DamageArgs
{
    public float damage;
    public GameObject damager;
    public Vector3 point;
    public Vector3 direction;
    public Vector3 normal;

    public DamageArgs(float damage, GameObject damager, Vector3 point) : this(damage, damager, point, (point - damager.transform.position).normalized) { }
    public DamageArgs(float damage, GameObject damager, Vector3 point, Vector3 direction) : this(damage, damager, point, direction, -direction) { }
    public DamageArgs(float damage, GameObject damager, Vector3 point, Vector3 direction, Vector3 normal)
    {
        this.damage = damage;
        this.damager = damager;
        this.point = point;
        this.direction = direction;
        this.normal = normal;
    }
}
