using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class WeaponPickup : MonoBehaviour
{
    private Weapon weapon = null;

    public void AssignWeapon(Weapon weapon)
    {
        this.weapon = weapon;
        GetComponent<SpriteRenderer>().sprite = weapon.WeaponInfo.pickupSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player is not null)
        {
            player.PickUpWeapon(weapon);
        }
    }
}
