using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;


[SelectionBase]
[DisallowMultipleComponent]
public sealed class PlayerWeaponManager : MonoBehaviour
{
    [SerializeField] Transform weaponParent;

    int switchLock;

    public int ActiveWeaponIndex { get; private set; }
    public Weapon ActiveWeapon => GetWeapon(ActiveWeaponIndex);

    public int LastWeaponIndex { get; private set; }
    public Weapon LastWeapon => GetWeapon(LastWeaponIndex);

    public Weapon GetWeapon(int i)
    {
        if (i < 0) return null;
        if (i >= weaponParent.childCount) return null;

        if (weaponParent.GetChild(i).TryGetComponent(out Weapon weapon))
        {
            return weapon;
        }
        else return null;
    }

    public void SwitchWeapon (int index)
    {
        if (ActiveWeapon) ActiveWeapon.Holster();
        ActiveWeaponIndex = index;
        if (ActiveWeapon) ActiveWeapon.Equip();
    }
}
