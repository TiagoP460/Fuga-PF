using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class SecurityManager : MonoBehaviour
{
    public static SecurityManager instance;

    [Header("Estado da Segurança")]
    [SerializeField] private bool securityDisabled = false;

    public bool IsSecurityDisabled
    {
        get { return securityDisabled; }
    }

    [Header("Alarme")]
    [SerializeField] private AudioSource alarmAudio;

    [Header("Tela Escura")]
    [SerializeField] private GameObject darkPanel;
    [SerializeField] private CanvasGroup darkCanvasGroup;
    [SerializeField] private float fadeDuration = 3f;
    [SerializeField] private float finalDarkAlpha = 0.85f;

    [Header("Luz Vermelha Piscando")]
    [SerializeField] private GameObject redFlashPanel;
    [SerializeField] private CanvasGroup redFlashCanvasGroup;
    [SerializeField] private float redFlashSpeed = 8f;
    [SerializeField] private float redFlashMaxAlpha = 0.45f;

    [Header("Tela Você Foi Pego")]
    [SerializeField] private GameObject caughtPanel;
    [SerializeField] private TextMeshProUGUI caughtText;
    [SerializeField] private GameObject returnMenuButton;

    [Header("Player")]
    [SerializeField] private GameObject playerObject;

    [Header("Menu Inicial")]
    [SerializeField] private string mainMenuSceneName = "MenuInicial";

    private Behaviour playerMovementScript;
    private Rigidbody2D playerRb;
    private Collider2D playerCollider;

    private bool alarmStarted = false;
    private Coroutine redFlashCoroutine;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        securityDisabled = false;
        alarmStarted = false;

        FindPlayerIfNeeded();
        SetupUI();

        Time.timeScale = 1f;
    }

    private void FindPlayerIfNeeded()
    {
        if (playerObject == null)
        {
            GameObject foundPlayer = GameObject.FindGameObjectWithTag("Player");

            if (foundPlayer != null)
            {
                playerObject = foundPlayer;
            }
        }

        if (playerObject != null)
        {
            playerMovementScript = playerObject.GetComponent<PlayerMovement>();
            playerRb = playerObject.GetComponent<Rigidbody2D>();
            playerCollider = playerObject.GetComponent<Collider2D>();
        }
    }

    private void SetupUI()
    {
        if (darkPanel != null)
        {
            darkPanel.SetActive(true);
        }

        if (darkCanvasGroup != null)
        {
            darkCanvasGroup.alpha = 0f;
            darkCanvasGroup.interactable = false;
            darkCanvasGroup.blocksRaycasts = false;
        }

        if (redFlashPanel != null)
        {
            redFlashPanel.SetActive(true);
        }

        if (redFlashCanvasGroup != null)
        {
            redFlashCanvasGroup.alpha = 0f;
            redFlashCanvasGroup.interactable = false;
            redFlashCanvasGroup.blocksRaycasts = false;
        }

        if (caughtPanel != null)
        {
            caughtPanel.SetActive(false);
        }

        if (caughtText != null)
        {
            caughtText.text = "Você foi pego";
        }

        if (returnMenuButton != null)
        {
            returnMenuButton.SetActive(false);
        }
    }

    public void DisableSecurity()
    {
        securityDisabled = true;
    }

    public void TriggerSecurityFail()
    {
        TriggerAlarm();
    }

    public void TriggerAlarm()
    {
        if (alarmStarted) return;

        alarmStarted = true;
        StartCoroutine(SecurityFailRoutine());
    }

    private IEnumerator SecurityFailRoutine()
    {
        FreezePlayer();

        if (alarmAudio != null)
        {
            alarmAudio.Play();
        }

        if (redFlashCoroutine != null)
        {
            StopCoroutine(redFlashCoroutine);
        }

        redFlashCoroutine = StartCoroutine(RedFlashRoutine());

        if (darkPanel != null)
        {
            darkPanel.SetActive(true);
        }

        if (darkCanvasGroup != null)
        {
            darkCanvasGroup.alpha = 0f;
            darkCanvasGroup.interactable = false;
            darkCanvasGroup.blocksRaycasts = false;
        }

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            if (darkCanvasGroup != null)
            {
                darkCanvasGroup.alpha = Mathf.Lerp(
                    0f,
                    finalDarkAlpha,
                    timer / fadeDuration
                );
            }

            yield return null;
        }

        if (darkCanvasGroup != null)
        {
            darkCanvasGroup.alpha = finalDarkAlpha;
            darkCanvasGroup.interactable = true;
            darkCanvasGroup.blocksRaycasts = true;
        }

        ShowCaughtScreen();
    }

    private IEnumerator RedFlashRoutine()
    {
        if (redFlashPanel != null)
        {
            redFlashPanel.SetActive(true);
        }

        while (alarmStarted)
        {
            float alpha = Mathf.PingPong(
                Time.unscaledTime * redFlashSpeed,
                redFlashMaxAlpha
            );

            if (redFlashCanvasGroup != null)
            {
                redFlashCanvasGroup.alpha = alpha;
            }

            yield return null;
        }

        if (redFlashCanvasGroup != null)
        {
            redFlashCanvasGroup.alpha = 0f;
        }
    }

    private void ShowCaughtScreen()
    {
        if (caughtPanel != null)
        {
            caughtPanel.SetActive(true);
        }

        if (caughtText != null)
        {
            caughtText.text = "Você foi pego";
        }

        if (returnMenuButton != null)
        {
            returnMenuButton.SetActive(true);
        }
    }

    private void FreezePlayer()
    {
        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
            playerRb.gravityScale = 0f;
            playerRb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        if (playerCollider != null)
        {
            playerCollider.enabled = false;
        }
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        alarmStarted = false;

        if (alarmAudio != null)
        {
            alarmAudio.Stop();
        }

        SceneManager.LoadScene(mainMenuSceneName);
    }
}