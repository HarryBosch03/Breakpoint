using System;
using Unity.Android.Types;
using Unity.Burst;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public abstract class Weapon : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform muzzle;
    [SerializeField] float equipTime;

    [Space]
    [SerializeField] float viewmodelFOV;

    IBipedal biped;
    float lastEnableTime;

    public Animator Animator => animator;
    public Transform Muzzle => muzzle;

    public bool PrimaryFire { get; set; }
    public bool SeccondaryFire { get; set; }
    public bool Reload { get; set; }

    public abstract void WeaponLoop();

    private void Awake()
    {
        biped = GetComponentInParent<IBipedal>();
    }

    private void OnEnable()
    {
        lastEnableTime = Time.time;
    }

    public void Update()
    {
        PlayerCamera.ViewmodelFOV = viewmodelFOV;

        if (Time.time > lastEnableTime + equipTime)
        {
            WeaponLoop();
        }

        float planarSpeed = biped.PlanarVelocity.magnitude;
        animator.SetFloat("planarSpeed", planarSpeed);

        animator.SetBool("grounded", biped.IsGrounded);
    }

    public void Holster()
    {
        gameObject.SetActive(false);
    }

    public void Equip()
    {
        gameObject.SetActive(true);
    }
}

[System.Serializable]
public class ProjectileWeaponEffect
{
    public Projectile projectilePrefab;
    public float damage;
    public float speed;

    public void Execute (Weapon weapon)
    {
        var projectileInstance = UnityEngine.Object.Instantiate(projectilePrefab, weapon.Muzzle.position, weapon.Muzzle.rotation);
        projectileInstance.Damage = damage;
        projectileInstance.Speed = speed;
    }
}

[Serializable]
public class SingleFireWeaponTrigger
{
    [SerializeField] float delay;

    float lastShootTime;
    bool lastState;

    public bool Check (bool inputState)
    {
        if (inputState && !lastState && Time.time > lastShootTime + delay)
        {
            return true;
        }

        lastState = inputState;
        return false;
    }

    public void OnFire ()
    {
        lastShootTime = Time.time;
    }
}

public class AimWeaponEffect
{
    [SerializeField] float aimSpeed;
    [SerializeField] float aimZoom;
    [SerializeField] float aimViewmodelFOV;
    [SerializeField] AnimationCurve aimCurve;

    float aimPercent;

    public void Loop (Weapon weapon, bool aimState)
    {
        aimPercent = Mathf.MoveTowards(aimPercent, aimState ? 1.0f : 0.0f, aimSpeed * Time.deltaTime);
        aimPercent = Mathf.Clamp01(aimPercent);
        weapon.Animator.SetFloat("aim", aimPercent);

        PlayerCamera.Zoom = Mathf.Lerp(PlayerCamera.Zoom, aimZoom, aimPercent);
        PlayerCamera.ViewmodelFOV = Mathf.Lerp(PlayerCamera.ViewmodelFOV, aimViewmodelFOV, aimPercent);
    }
}