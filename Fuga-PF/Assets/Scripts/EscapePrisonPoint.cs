using System.Collections;
using UnityEngine;
using TMPro;

public class EscapePrisonPoint : MonoBehaviour
{
    [Header("Boss")]
    [SerializeField] private Health lizardBossHealth;

    [Header("Interação")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Mensagens")]
    [SerializeField] private string blockedMessage = "Derrote o lagarto antes de escapar.";
    [SerializeField] private string escapeMessage = "Aperte E para escapar da prisão.";

    [Header("Final da Fase")]
    [SerializeField] private CanvasGroup finalFadeCanvasGroup;
    [SerializeField] private GameObject finalFadePanel;
    [SerializeField] private TextMeshProUGUI finalText;
    [SerializeField] private float fadeDuration = 3f;

    [Header("Player")]
    [SerializeField] private Behaviour playerMovementScript;

    private bool playerNear = false;
    private bool endingStarted = false;

    private Rigidbody2D playerRb;
    private Collider2D playerCollider;

    private void Start()
    {
        if (finalFadePanel != null)
        {
            finalFadePanel.SetActive(true);
        }

        if (finalFadeCanvasGroup != null)
        {
            finalFadeCanvasGroup.alpha = 0f;
            finalFadeCanvasGroup.interactable = false;
            finalFadeCanvasGroup.blocksRaycasts = false;
        }

        if (finalText != null)
        {
            finalText.gameObject.SetActive(false);
            finalText.text = "Você conseguiu escapar da prisão!";
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

        playerRb = collision.GetComponent<Rigidbody2D>();
        playerCollider = collision;

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

        if (finalText != null)
        {
            finalText.gameObject.SetActive(true);
            finalText.text = "Você conseguiu escapar da prisão!";
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