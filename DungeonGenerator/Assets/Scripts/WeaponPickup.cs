using UnityEngine;

[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(SpriteRenderer))]

public class WeaponPickup : MonoBehaviour
{
    [SerializeField] WeaponInfo weaponInfo;

    private void Awake()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = weaponInfo.pickupSprite;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();

        if (player is not null)
        {
            player.PickUpWeapon(weaponInfo);
        }
    }
}
