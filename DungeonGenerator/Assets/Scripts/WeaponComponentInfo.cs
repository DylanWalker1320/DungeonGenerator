using UnityEngine;

public class WeaponComponentInfo
{
    public string componentName { get; private set; }
    public string description { get; private set; }

    public WeaponComponentInfo(string name, string description)
    {
        this.componentName = name;
        this.description = description;
    }

    public override string ToString()
    {
        return $"<{componentName} : {description}> ";
    }
}
