using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

[System.Serializable]
public sealed class PlayerWeaponManager : NestedBehaviour
{
    Transform weaponParent;

    bool switchLock;

    public int ActiveWeaponIndex { get; private set; }
    public Weapon ActiveWeapon => GetWeapon(ActiveWeaponIndex);

    public int LastWeaponIndex { get; private set; }
    public Weapon LastWeapon => GetWeapon(LastWeaponIndex);

    public void Initalize(MonoBehaviour caller, Transform weaponParent, Transform camRotor, PlayerCamera pcam)
    {
        this.weaponParent = weaponParent;

        for (int i = 0; i < weaponParent.childCount; i++)
        {
            var weapon = weaponParent.GetChild(i).GetComponent<Weapon>();
            weapon.gameObject.SetActive(false);
            weapon.Initalize(camRotor, pcam);
        }

        ActiveWeaponIndex = -1;
        SwitchWeapon(caller, 0);
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
        if (i < 0) return false;
        if (i >= weaponParent.childCount) return false;

        return true;
    }

    public void SwitchWeapon(MonoBehaviour caller, int i)
    {
        caller.StartCoroutine(SwitchWeaponRoutine(caller, i));
    }

    public IEnumerator SwitchWeaponRoutine(MonoBehaviour caller, int i)
    {
        if (!IsWeaponIndexValid(i) && i != -1) yield break;
        if (i == ActiveWeaponIndex) yield break;

        if (switchLock) yield break;
        switchLock = true;

        if (ActiveWeapon)
        {
            var action = ActiveWeapon.Holster();
            if (action != null) yield return caller.StartCoroutine(action);
        }

        ActiveWeaponIndex = i;
        Debug.Log(ActiveWeaponIndex);
        if (ActiveWeapon)
        {
            var action = ActiveWeapon.Equip();
            if (action != null) yield return caller.StartCoroutine(action);
        }

        switchLock = false;
    }

    public void Process(MonoBehaviour caller, bool primaryFire, bool seccondaryFire, bool reload, int selectedWeapon)
    {
        SwitchWeapon(caller, selectedWeapon);

        if (ActiveWeapon)
        {
            ActiveWeapon.PrimaryFire = primaryFire;
            ActiveWeapon.SeccondaryFire = seccondaryFire;
            ActiveWeapon.Reload = reload;
        }
    }
}
