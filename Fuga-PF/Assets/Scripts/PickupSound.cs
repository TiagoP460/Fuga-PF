using UnityEngine;

public class PickupSound : MonoBehaviour
{
    [Header("Som ao pegar item")]
    public AudioClip pickupSound;

    [Range(0f, 1f)]
    public float volume = 1f;

    private void Awake()
    {
        AudioSource audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.loop = false;
            audioSource.Stop();
        }
    }

    public void PlayPickupSound()
    {
        if (pickupSound == null)
        {
            Debug.LogWarning("Coloque o arquivo de som no campo Pickup Sound do PickupSound.");
            return;
        }

        GameObject soundObject = new GameObject("PickupSoundTemp");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();

        audioSource.clip = pickupSound;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;
        audioSource.loop = false;

        audioSource.Play();

        Destroy(soundObject, pickupSound.length);
    }
}