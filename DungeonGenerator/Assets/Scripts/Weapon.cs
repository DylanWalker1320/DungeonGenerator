using System.Collections.Generic;
using UnityEngine;

public class Weapon
{
    public WeaponInfo WeaponInfo { get; private set; }
    public List<WeaponComponent> Components { get; private set; }

    public Weapon(WeaponInfo weaponInfo, List<WeaponComponent> components)
    {
        this.WeaponInfo = weaponInfo;
        this.Components = components;
    }
}
