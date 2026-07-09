using UnityEngine;

public class AccessCardDoor : MonoBehaviour
{
    [Header("Estado da porta")]
    public bool opened = false;

    [Header("Som da porta")]
    public AudioClip openDoorSound;

    [Range(0f, 1f)]
    public float openDoorVolume = 1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (opened) return;

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
        if (opened) return;

        opened = true;

        if (MessageManager.instance != null)
        {
            MessageManager.instance.ShowMessage("Cartão aceito! Porta aberta.");
        }

        PlayOpenDoorSound();

        Destroy(gameObject);
    }

    private void PlayOpenDoorSound()
    {
        if (openDoorSound == null)
        {
            Debug.LogWarning("Coloque o som da porta no campo Open Door Sound.");
            return;
        }

        GameObject soundObject = new GameObject("AccessCardDoorOpenSound");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();

        audioSource.clip = openDoorSound;
        audioSource.volume = openDoorVolume;
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        audioSource.Play();

        Destroy(soundObject, openDoorSound.length);
    }
}