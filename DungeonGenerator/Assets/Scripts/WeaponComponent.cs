using UnityEngine;

public abstract class WeaponComponent
{
    WeaponComponentInfo weaponComponentInfo;
    public WeaponComponent(WeaponComponentInfo info)
    {
        this.weaponComponentInfo = info;
    }
    public abstract void Apply();
    public abstract void Unapply();

    public WeaponComponentInfo GetInfo()
    {
        return weaponComponentInfo;
    }
}
