using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                inventory.hasKey = true;

                if (MessageManager.instance != null)
                {
                    MessageManager.instance.ShowMessage("Chave coletada!");
                }

                Destroy(gameObject);
            }
        }
    }
}