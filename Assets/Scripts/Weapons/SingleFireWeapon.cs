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

    public override void WeaponLoop()
    {
        CheckPrimaryFire();
        CheckSeccondaryFire();
    }

    private void CheckPrimaryFire()
    {
        if (!trigger.Check(PrimaryFire)) return;

        effect.Execute(this);
        trigger.OnFire();

        Animator.Play("Fire");
    }

    private void CheckSeccondaryFire()
    {
        aiming.Loop(this, SeccondaryFire);
    }
}
