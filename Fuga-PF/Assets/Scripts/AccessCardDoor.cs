using UnityEngine;

public class AccessCardDoor : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.collider.GetComponent<PlayerInventory>();

            if (inventory != null && inventory.hasAccessCard)
            {
                OpenDoor();
            }
            else
            {
                if (MessageManager.instance != null)
                {
                    MessageManager.instance.ShowMessage("A porta está trancada. Encontre o cartão de acesso!");
                }
            }
        }
    }

    void OpenDoor()
    {
        if (MessageManager.instance != null)
        {
            MessageManager.instance.ShowMessage("Cartão aceito! Porta aberta.");
        }

        Destroy(gameObject);
    }
}