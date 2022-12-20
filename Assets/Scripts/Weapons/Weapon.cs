using System;
using Unity.Burst;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public abstract class Weapon : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] Transform muzzle;
    [SerializeField] float equipTime;
    [SerializeField] Transform root;

    [Space]
    [SerializeField] float viewmodelFOV;

    IBipedal biped;
    float lastEnableTime;

    public Animator Animator => animator;
    public Transform Muzzle => muzzle;
    public Transform Root => root;

    public bool PrimaryFire { get; set; }
    public bool SeccondaryFire { get; set; }
    public bool Reload { get; set; }

    public abstract void WeaponLoop();

    protected virtual void Awake()
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

    public Coroutine Holster()
    {
        PrimaryFire = false;
        SeccondaryFire = false;
        Reload = false;

        gameObject.SetActive(false);

        return null;
    }

    public Coroutine Equip()
    {
        gameObject.SetActive(true);

        return StartCoroutine(CoroutineUtility.Wait(equipTime));
    }
}

[System.Serializable]
public class ProjectileWeaponEffect
{
    public Projectile projectilePrefab;
    public float damage;
    public float speed;
    public ParticleSystem[] fireFX;

    public void Execute (Weapon weapon)
    {
        GameObject shooter = weapon.gameObject;
        var avatarRoot = shooter.GetComponentInParent<IAvatarRoot>();
        if (avatarRoot != null)
        {
            shooter = avatarRoot.gameObject;
        }
        else
        {
            var rigidbody = shooter.GetComponentInParent<Rigidbody>();
            if (rigidbody)
            {
                shooter = rigidbody.gameObject;
            }
        }

        var projectileInstance = UnityEngine.Object.Instantiate(projectilePrefab, weapon.Muzzle.position, weapon.Muzzle.rotation);
        projectileInstance.Damage = damage;
        projectileInstance.Speed = speed;
        projectileInstance.Shooter = shooter;
        foreach (var fx in fireFX) fx.Play();
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


[Serializable]
public class AimWeaponEffect
{
    [SerializeField] float aimSpeed;
    [SerializeField] float aimZoom;
    [SerializeField] float aimViewmodelFOV;
    [SerializeField] AnimationCurve aimCurve;

    IBipedal biped;
    float aimPercent;
    
    public void Loop (Weapon weapon, bool aimState)
    {
        if (biped == null)
        {
            biped = weapon.GetComponentInParent<IBipedal>();
        }
        bool grounded = biped != null ? biped.IsGrounded : true;

        aimPercent = Mathf.MoveTowards(aimPercent, (aimState && grounded) ? 1.0f : 0.0f, aimSpeed * Time.deltaTime);
        aimPercent = Mathf.Clamp01(aimPercent);
        weapon.Animator.SetFloat("aim", aimPercent);

        PlayerCamera.SetZoom(aimZoom);
        PlayerCamera.FOVOverrideBlend = aimPercent;
        PlayerCamera.ViewmodelFOV = Mathf.Lerp(PlayerCamera.ViewmodelFOV, aimViewmodelFOV, aimPercent);
    }
}