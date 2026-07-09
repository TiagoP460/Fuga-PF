using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class EscapePrisonPoint : MonoBehaviour
{
    [Header("Boss")]
    public GameObject lizardBossObject;

    [Header("Interação")]
    public KeyCode interactKey = KeyCode.E;

    [Header("Mensagens")]
    public string blockedMessage = "Derrote o lagarto antes de escapar.";
    public string escapeMessage = "Aperte E para escapar da prisão.";

    [Header("Final da Fase")]
    public GameObject finalFadePanel;
    public GameObject finalTextObject;
    public GameObject finalMenuButton;
    public float fadeDuration = 3f;

    [Header("Menu")]
    public string mainMenuSceneName = "MenuInicial";

    [Header("Player")]
    public GameObject playerObject;

    private Health lizardBossHealth;
    private CanvasGroup finalFadeCanvasGroup;
    private TextMeshProUGUI finalText;
    private Behaviour playerMovementScript;
    private Rigidbody2D playerRb;
    private Collider2D playerCollider;

    private bool playerNear = false;
    private bool endingStarted = false;

    private void Start()
    {
        if (lizardBossObject != null)
        {
            lizardBossHealth = lizardBossObject.GetComponent<Health>();
        }

        if (finalFadePanel != null)
        {
            finalFadeCanvasGroup = finalFadePanel.GetComponent<CanvasGroup>();
            finalFadePanel.SetActive(true);
        }

        if (finalTextObject != null)
        {
            finalText = finalTextObject.GetComponent<TextMeshProUGUI>();
            finalTextObject.SetActive(false);

            if (finalText != null)
            {
                finalText.text = "Você conseguiu escapar da prisão!";
            }
        }

        if (finalMenuButton != null)
        {
            finalMenuButton.SetActive(false);
        }

        if (finalFadeCanvasGroup != null)
        {
            finalFadeCanvasGroup.alpha = 0f;
            finalFadeCanvasGroup.interactable = false;
            finalFadeCanvasGroup.blocksRaycasts = false;
        }

        if (playerObject != null)
        {
            playerMovementScript = playerObject.GetComponent<PlayerMovement>();
            playerRb = playerObject.GetComponent<Rigidbody2D>();
            playerCollider = playerObject.GetComponent<Collider2D>();
        }

        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (!playerNear)
            return;

        if (endingStarted)
            return;

        if (Input.GetKeyDown(interactKey))
        {
            if (IsBossDead())
            {
                StartCoroutine(EscapeEndingRoutine());
            }
            else
            {
                ShowMessage(blockedMessage);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerNear = true;

        if (playerObject == null)
        {
            playerObject = collision.gameObject;
            playerMovementScript = playerObject.GetComponent<PlayerMovement>();
            playerRb = playerObject.GetComponent<Rigidbody2D>();
            playerCollider = playerObject.GetComponent<Collider2D>();
        }

        if (endingStarted)
            return;

        if (IsBossDead())
        {
            ShowMessage(escapeMessage);
        }
        else
        {
            ShowMessage(blockedMessage);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerNear = false;

        if (!endingStarted)
        {
            ShowMessage("");
        }
    }

    private bool IsBossDead()
    {
        if (lizardBossObject == null)
            return true;

        if (lizardBossHealth == null)
            lizardBossHealth = lizardBossObject.GetComponent<Health>();

        if (lizardBossHealth == null)
            return true;

        return lizardBossHealth.currentHealth <= 0;
    }

    private IEnumerator EscapeEndingRoutine()
    {
        endingStarted = true;

        ShowMessage("");

        FreezePlayer();

        Time.timeScale = 0f;

        if (finalFadePanel != null)
        {
            finalFadePanel.SetActive(true);
        }

        if (finalFadeCanvasGroup != null)
        {
            finalFadeCanvasGroup.alpha = 0f;
            finalFadeCanvasGroup.interactable = true;
            finalFadeCanvasGroup.blocksRaycasts = true;
        }

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            if (finalFadeCanvasGroup != null)
            {
                finalFadeCanvasGroup.alpha = Mathf.Lerp(
                    0f,
                    1f,
                    timer / fadeDuration
                );
            }

            yield return null;
        }

        if (finalFadeCanvasGroup != null)
        {
            finalFadeCanvasGroup.alpha = 1f;
        }

        if (finalTextObject != null)
        {
            finalTextObject.SetActive(true);
        }

        if (finalText != null)
        {
            finalText.text = "Você conseguiu escapar da prisão!";
        }

        if (finalMenuButton != null)
        {
            finalMenuButton.SetActive(true);
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
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void ShowMessage(string message)
    {
        if (MessageManager.instance != null)
        {
            MessageManager.instance.ShowMessage(message);
        }
        else
        {
            Debug.Log(message);
        }
    }
}