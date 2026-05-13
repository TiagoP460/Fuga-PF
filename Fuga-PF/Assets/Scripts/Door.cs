using UnityEngine;

public class Door : MonoBehaviour
{
    public bool opened = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.collider.GetComponent<PlayerInventory>();

            if (inventory != null && inventory.hasKey)
            {
                OpenDoor();
            }
            else
            {
                if (MessageManager.instance != null)
                {
                    MessageManager.instance.ShowMessage("A porta está trancada. Encontre a chave.");
                }
            }
        }
    }

    void OpenDoor()
    {
        opened = true;

        if (MessageManager.instance != null)
        {
            MessageManager.instance.ShowMessage("Porta aberta!");
        }

        Destroy(gameObject);
    }
}