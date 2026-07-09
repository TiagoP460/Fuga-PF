using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private bool collected = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collected) return;

        if (!collision.CompareTag("Player"))
            return;

        PlayerInventory inventory = collision.GetComponent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogWarning("O Player não tem PlayerInventory.");
            return;
        }

        collected = true;

        inventory.hasKey = true;

        if (MessageManager.instance != null)
        {
            MessageManager.instance.ShowMessage("Chave coletada!");
        }

        PickupSound pickupSound = GetComponent<PickupSound>();

        if (pickupSound != null)
        {
            pickupSound.PlayPickupSound();
        }
        else
        {
            Debug.LogWarning("A chave não tem o script PickupSound.");
        }

        Destroy(gameObject);
    }
}