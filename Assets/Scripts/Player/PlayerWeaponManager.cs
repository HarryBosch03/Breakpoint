using System;
using System.Collections;
using UnityEngine;

[System.Serializable]
public sealed class PlayerWeaponManager : NestedBehaviour
{
    Transform weaponParent;

    Func<bool> getPrimaryFire;
    Func<bool> getSeccondaryFire;
    Func<bool> getReload;

    bool switchLock;

    public int ActiveWeaponIndex { get; private set; }
    public Weapon ActiveWeapon => GetWeapon(ActiveWeaponIndex);

    public int LastWeaponIndex { get; private set; }
    public Weapon LastWeapon => GetWeapon(LastWeaponIndex);

    public PlayerWeaponManager(MonoBehaviour context, Transform weaponParent, Func<bool> getPrimaryFire, Func<bool> getSeccondaryFire, Func<bool> getReload) : base(context) 
    {
        this.weaponParent = weaponParent;
        this.getPrimaryFire = getPrimaryFire;
        this.getSeccondaryFire = getSeccondaryFire;
        this.getReload = getReload;
    }

    public Weapon GetWeapon(int i)
    {
        if (!IsWeaponIndexValid(i)) return null;

        if (weaponParent.GetChild(i).TryGetComponent(out Weapon weapon))
        {
            return weapon;
        }
        else return null;
    }

    public bool IsWeaponIndexValid(int i)
    {
        if (i == -1) return true;

        if (i < 0) return false;
        if (i > weaponParent.childCount) return false;

        return true;
    }

    public void SwitchWeapon(int i)
    {
        Context.StartCoroutine(SwitchWeaponRoutine(i));
    }

    public IEnumerator SwitchWeaponRoutine(int i)
    {
        if (!IsWeaponIndexValid(i)) yield break;

        if (switchLock) yield break;
        switchLock = true;

        if (ActiveWeapon)
        {
            yield return ActiveWeapon.Holster();
        }

        ActiveWeaponIndex = i;
        if (ActiveWeapon)
        {
            yield return ActiveWeapon.Equip();
        }

        switchLock = false;
    }

    protected override void OnExecute()
    {
        if (ActiveWeapon)
        {
            ActiveWeapon.PrimaryFire = getPrimaryFire();
            ActiveWeapon.SeccondaryFire = getSeccondaryFire();
            ActiveWeapon.Reload = getReload();
        }
    }
}
