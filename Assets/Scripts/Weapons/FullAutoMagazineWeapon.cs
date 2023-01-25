using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

[SelectionBase]
[DisallowMultipleComponent]
public sealed class FullAutoMagazineWeapon : Weapon
{
    [Space]
    [Header("Semi Magazine Weapon")]
    [SerializeField] ProjectileWeaponEffect effect;
    [SerializeField] FullAutoWeaponTrigger trigger;
    [SerializeField] AimWeaponEffect aiming;
    [SerializeField] WeaponSway sway;
    [SerializeField] WeaponRedicle redicle;
    [SerializeField] WeaponMagazineCondition magazine;
    [SerializeField] float defaultRedicleSize;
    [SerializeField] RecoilWeaponEffect recoil;

    protected override void Awake()
    {
        base.Awake();

        sway.Root = Root;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        magazine.OnEnable();
    }

    public override void WeaponLoop()
    {
        CheckPrimaryFire();
        CheckSeccondaryFire();
        CheckReload();
    }

    private void LateUpdate()
    {
        sway.LateLoop();

        redicle.FadeAmount = 1.0f - aiming.AimPercent;
        redicle.Size = Mathf.Lerp(defaultRedicleSize, 0.0f, aiming.AimPercent);
    }

    private void CheckPrimaryFire()
    {
        if (!trigger.Check(PrimaryFire)) return;

        if (!magazine.CanFire())
        {
            magazine.Reload(this);
            return;
        }

        effect.Execute(this);
        trigger.OnFire();
        magazine.OnFire();

        if (Animator.isActiveAndEnabled) Animator.Play("Fire", 0, 0.0f);
        recoil.AddRecoil(pcam);
    }

    private void CheckSeccondaryFire()
    {
        aiming.Loop(this, SeccondaryFire && !magazine.IsReloading, pcam);
    }

    private void CheckReload()
    {
        if (!Reload) return;

        magazine.Reload(this);
    }
}
