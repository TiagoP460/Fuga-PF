using UnityEngine;

public class PickupSound : MonoBehaviour
{
    [Header("Som ao pegar item")]
    public AudioClip pickupSound;

    [Range(0f, 1f)]
    public float volume = 1f;

    public void PlayPickupSound()
    {
        if (pickupSound == null)
            return;

        GameObject soundObject = new GameObject("PickupSound");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();

        audioSource.clip = pickupSound;
        audioSource.volume = volume;
        audioSource.spatialBlend = 0f;
        audioSource.playOnAwake = false;

        audioSource.Play();

        Destroy(soundObject, pickupSound.length);
    }
}