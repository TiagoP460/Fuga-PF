using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GlobalButtonSound : MonoBehaviour
{
    public static GlobalButtonSound instance;

    [Header("Som dos Botões")]
    [SerializeField] private AudioClip buttonClickSound;
    [Range(0f, 1f)]
    [SerializeField] private float buttonVolume = 1f;

    private AudioSource audioSource;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        RegisterButtonsInScene();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RegisterButtonsInScene();
    }

    private void RegisterButtonsInScene()
    {
        Button[] buttons = FindObjectsByType<Button>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (Button button in buttons)
        {
            button.onClick.RemoveListener(PlayButtonClick);
            button.onClick.AddListener(PlayButtonClick);
        }
    }

    public void PlayButtonClick()
    {
        if (audioSource != null && buttonClickSound != null)
        {
            audioSource.PlayOneShot(buttonClickSound, buttonVolume);
        }
    }
}