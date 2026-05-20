using UnityEngine;

public class GunPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerInventory inventory = collision.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                inventory.hasGun = true;

                Debug.Log("Arma coletada! hasGun = " + inventory.hasGun);

                if (MessageManager.instance != null)
                {
                    MessageManager.instance.ShowMessage("Arma coletada! Use o botão direito para atirar.");
                }

                Destroy(gameObject);
            }
        }
    }
}