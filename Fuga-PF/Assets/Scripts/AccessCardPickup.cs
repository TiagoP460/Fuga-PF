using UnityEngine;

public class AccessCardPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                inventory.hasAccessCard = true;

                if (MessageManager.instance != null)
                {
                    MessageManager.instance.ShowMessage("Cartão de acesso coletado!");
                }

                Destroy(gameObject);
            }
        }
    }
}