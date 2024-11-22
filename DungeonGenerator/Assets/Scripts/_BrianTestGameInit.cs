using System.Collections.Generic;
using UnityEngine;

public class _BrianTestGameInit : MonoBehaviour
{
    [SerializeField] GameObject weaponPickupPrefab;
    [SerializeField] Player player;
    [SerializeField] WeaponInfo punchInfo;
    [SerializeField] WeaponInfo kickInfo;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Instantiate(weaponPickupPrefab, new Vector3(-4, -4), Quaternion.identity)
            .GetComponent<WeaponPickup>()
            .AssignWeapon(new Weapon(punchInfo, new List<WeaponComponent>() { new SpeedIncreaseComponent(player) }));

        Instantiate(weaponPickupPrefab, new Vector3(4, -4), Quaternion.identity)
            .GetComponent<WeaponPickup>()
            .AssignWeapon(new Weapon(kickInfo, new List<WeaponComponent>() { new AttackCountComponent(player) }));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
