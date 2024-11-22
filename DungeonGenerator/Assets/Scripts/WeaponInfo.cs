using UnityEngine;

[CreateAssetMenu(fileName = "WeaponInfo", menuName = "Scriptable Objects/WeaponInfo")]
public class WeaponInfo : ScriptableObject
{
    [field: SerializeField] public Sprite pickupSprite { get; private set; }
    [field: SerializeField] public string weaponName { get; private set; }
    [field: SerializeField] public string description { get; private set; }
    [field: SerializeField] public AnimatorOverrideController attackOverrideController { get; private set; }
}
