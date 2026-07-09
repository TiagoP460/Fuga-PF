using UnityEngine;

public class GunPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        PlayerInventory inventory = collision.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.hasGun = true;
        }

        PickupSound pickupSound = GetComponent<PickupSound>();

        if (pickupSound != null)
        {
            pickupSound.PlayPickupSound();
        }

        Destroy(gameObject);
    }
}