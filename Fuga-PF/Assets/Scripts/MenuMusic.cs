using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    [Header("Música do Menu")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private float volume = 0.5f;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.clip = menuMusic;
        audioSource.volume = volume;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    private void Start()
    {
        if (menuMusic != null)
        {
            audioSource.Play();
        }
    }
}