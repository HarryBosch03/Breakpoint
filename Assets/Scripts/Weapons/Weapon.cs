using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

[SelectionBase]
[DisallowMultipleComponent]
public abstract class Weapon : MonoBehaviour
{
    [SerializeField] float equipTime;
    [SerializeField] Transform root;
    [SerializeField] Animator animator;

    [Space]
    [SerializeField] float viewmodelFOV;

    [Space]
    [SerializeField] GameObject model;
    [SerializeField] int viewmodelOwnerLayer = 7;

    IBipedal biped;
    Transform camRotor;
    float lastEnableTime;
    protected PlayerCamera pcam;

    public Animator Animator => animator;
    public Transform CameraRotor => camRotor;
    public Transform Root => root;

    public bool PrimaryFire { get; set; }
    public bool SeccondaryFire { get; set; }
    public bool Reload { get; set; }

    public abstract void WeaponLoop();

    protected virtual void Awake()
    {
        biped = GetComponentInParent<IBipedal>();
    }

    protected virtual void OnEnable()
    {
        lastEnableTime = Time.time;
    }

    private void Start()
    {
        var networkObject = GetComponentInParent<NetworkObject>();
        if (networkObject)
        {
            model.layer = networkObject.IsOwner ? viewmodelOwnerLayer : 0;
        }
    }

    public void Initalize(Transform camRotor, PlayerCamera pcam)
    {
        this.camRotor = camRotor;
        this.pcam = pcam;
    }

    public void Update()
    {
        pcam.ViewmodelFOV = viewmodelFOV;

        if (Time.time > lastEnableTime + equipTime)
        {
            WeaponLoop();
        }

        float planarSpeed = biped.PlanarVelocity.magnitude;
        animator.SetFloat("planarSpeed", planarSpeed);

        animator.SetBool("grounded", biped.IsGrounded);
    }

    public IEnumerator Holster()
    {
        PrimaryFire = false;
        SeccondaryFire = false;
        Reload = false;

        gameObject.SetActive(false);

        return null;
    }

    public IEnumerator Equip()
    {
        gameObject.SetActive(true);

        return CoroutineUtility.Wait(equipTime);
    }
}

[System.Serializable]
public class ProjectileWeaponEffect
{
    public Projectile projectilePrefab;
    public float damage;
    public float speed;
    public ParticleSystem[] fireFX;

    public void Execute(Weapon weapon)
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

        var projectileInstance = UnityEngine.Object.Instantiate(projectilePrefab, weapon.CameraRotor.position, weapon.CameraRotor.rotation);
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

    public bool Check(bool inputState)
    {
        if (inputState && !lastState && Time.time > lastShootTime + delay)
        {
            return true;
        }

        lastState = inputState;
        return false;
    }

    public void OnFire()
    {
        lastShootTime = Time.time;
    }
}

[Serializable]
public class FullAutoWeaponTrigger
{
    [SerializeField] float fireRate;

    float lastShootTime;

    public bool Check(bool inputState)
    {
        if (inputState && Time.time > lastShootTime + 60.0f / fireRate)
        {
            return true;
        }

        return false;
    }

    public void OnFire()
    {
        lastShootTime = Time.time;
    }
}

[System.Serializable]
public class WeaponMagazineCondition
{
    [SerializeField] int magazineSize;
    [SerializeField] int currentMagazine;

    [Space]
    [SerializeField] float reloadTime;

    public bool IsReloading { get; private set; }

    public void OnEnable()
    {
        IsReloading = false;
    }

    public bool CanFire()
    {
        if (IsReloading) return false;
        if (currentMagazine <= 0) return false;

        return true;
    }

    public void OnFire()
    {
        currentMagazine--;
    }

    public void Reload(Weapon caller)
    {
        caller.StartCoroutine(ReloadRoutine(caller));
    }

    private IEnumerator ReloadRoutine(Weapon caller)
    {
        if (IsReloading) yield break;

        IsReloading = true;
        currentMagazine = 0;

        if (caller.Animator.isActiveAndEnabled) caller.Animator.Play("Reload", 0, 0.0f);

        yield return new WaitForSeconds(reloadTime);

        currentMagazine = magazineSize;
        IsReloading = false;
    }
}


[Serializable]
public class AimWeaponEffect
{
    [SerializeField] float aimSpeed;
    [SerializeField] float aimZoom;
    [SerializeField] float aimViewmodelFOV;
    [SerializeField] AnimationCurve aimCurve;

    float aimPercent;

    public float AimPercent => aimCurve.Evaluate(aimPercent);

    public void Loop(Weapon weapon, bool aimState, PlayerCamera camera)
    {
        aimPercent = Mathf.MoveTowards(aimPercent, (aimState) ? 1.0f : 0.0f, aimSpeed * Time.deltaTime);
        aimPercent = Mathf.Clamp01(aimPercent);
        if (weapon.Animator.isActiveAndEnabled)
        {
            weapon.Animator.SetFloat("aim", AimPercent);
            weapon.Animator.SetBool("aimState", aimState);
        }

        camera.SetZoom(aimZoom);
        camera.FOVOverrideBlend = aimPercent;
        camera.ViewmodelFOV = Mathf.Lerp(camera.ViewmodelFOV, aimViewmodelFOV, aimPercent);
    }
}

[System.Serializable]
public class WeaponRedicle
{
    [SerializeField] CanvasGroup group;

    public float FadeAmount { get => group.alpha; set => group.alpha = value; }
    public float Size
    {
        get => ((RectTransform)group.transform).rect.width;
        set
        {
            var transform = group.transform as RectTransform;
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, value);
            transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, value);
        }
    }
}

[System.Serializable]
public class RecoilWeaponEffect
{
    [SerializeField] Vector2 recoilRangeX;
    [SerializeField] Vector2 recoilRangeY;
    [SerializeField] float recoilMagnitude;

    public void AddRecoil(PlayerCamera camera)
    {
        camera.AddRecoil(new Vector2(Mathf.Lerp(recoilRangeX.x, recoilRangeX.y, UnityEngine.Random.value), Mathf.Lerp(recoilRangeY.x, recoilRangeY.y, UnityEngine.Random.value)) * recoilMagnitude);
    }
}