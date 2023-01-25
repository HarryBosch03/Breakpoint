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
    [SerializeField] WeaponRedicle redicle;
    [SerializeField] float defaultRedicleSize;
    [SerializeField] RecoilWeaponEffect recoil;

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
        redicle.FadeAmount = 1.0f - aiming.AimPercent;
        redicle.Size = Mathf.Lerp(defaultRedicleSize, 0.0f, aiming.AimPercent);
    }

    private void CheckPrimaryFire()
    {
        if (!trigger.Check(PrimaryFire)) return;

        effect.Execute(this);
        trigger.OnFire();
        recoil.AddRecoil(pcam);

        if (Animator.isActiveAndEnabled) Animator.Play("Fire", 0, 0.0f);
    }

    private void CheckSeccondaryFire()
    {
        aiming.Loop(this, SeccondaryFire, pcam);
    }
}
