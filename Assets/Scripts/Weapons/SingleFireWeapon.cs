using System;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class SingleFireWeapon : Weapon
{
    [Space]
    [Header("SINGLE FIRE WEAPON")]
    [SerializeField] ProjectileWeaponEffect effect;
    [SerializeField] SingleFireWeaponTrigger trigger;
    [SerializeField] AimWeaponEffect aiming;
    [SerializeField] WeaponSway sway;

    protected override void Awake()
    {
        base.Awake();

        sway.Root = Root;
    }

    public override void WeaponLoop()
    {
        CheckPrimaryFire();
        CheckSeccondaryFire();
    }

    private void LateUpdate()
    {
        sway.LateLoop();
    }

    private void CheckPrimaryFire()
    {
        if (!trigger.Check(PrimaryFire)) return;

        effect.Execute(this);
        trigger.OnFire();

        Animator.Play("Fire", 0, 0.0f);
    }

    private void CheckSeccondaryFire()
    {
        aiming.Loop(this, SeccondaryFire);
    }
}
