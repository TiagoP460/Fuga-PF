using UnityEngine;

public class KeyItem : MonoBehaviour
{
    [Header("Som ao pegar a chave")]
    public AudioClip pickupSound;

    [Range(0f, 1f)]
    public float pickupVolume = 1f;

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

        PlayPickupSound();

        Destroy(gameObject);
    }

    private void PlayPickupSound()
    {
        AudioClip clipToPlay = pickupSound;

        if (clipToPlay == null)
        {
            AudioSource audioSourceOnItem = GetComponent<AudioSource>();

            if (audioSourceOnItem != null)
            {
                clipToPlay = audioSourceOnItem.clip;
            }
        }

        if (clipToPlay == null)
        {
            Debug.LogWarning("Nenhum som foi encontrado na chave. Coloque o áudio no campo Pickup Sound ou no AudioSource da chave.");
            return;
        }

        GameObject soundObject = new GameObject("KeyPickupSound");
        AudioSource tempAudioSource = soundObject.AddComponent<AudioSource>();

        tempAudioSource.clip = clipToPlay;
        tempAudioSource.volume = pickupVolume;
        tempAudioSource.spatialBlend = 0f;
        tempAudioSource.playOnAwake = false;

        tempAudioSource.Play();

        Destroy(soundObject, clipToPlay.length);

        Debug.Log("Som da chave tocou.");
    }
}