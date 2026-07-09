using System.Collections;
using UnityEngine;
using TMPro;

public class PhaseIntroController : MonoBehaviour
{
    [Header("Painel preto")]
    [SerializeField] private GameObject introPanel;
    [SerializeField] private CanvasGroup introCanvasGroup;

    [Header("Texto da introdução")]
    [SerializeField] private TextMeshProUGUI introText;
    [TextArea]
    [SerializeField] private string introMessage = "Encare o desafio final para fugir...";

    [Header("Tempo")]
    [SerializeField] private float messageTime = 2.5f;
    [SerializeField] private float fadeDuration = 2f;

    [Header("Player")]
    [SerializeField] private Behaviour playerMovementScript;

    private void Start()
    {
        StartCoroutine(IntroRoutine());
    }

    private IEnumerator IntroRoutine()
    {
        Time.timeScale = 0f;

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }

        if (introPanel != null)
        {
            introPanel.SetActive(true);
        }

        if (introCanvasGroup != null)
        {
            introCanvasGroup.alpha = 1f;
            introCanvasGroup.interactable = true;
            introCanvasGroup.blocksRaycasts = true;
        }

        if (introText != null)
        {
            introText.gameObject.SetActive(true);
            introText.text = introMessage;
            introText.alpha = 1f;
        }

        yield return new WaitForSecondsRealtime(messageTime);

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            if (introCanvasGroup != null)
            {
                introCanvasGroup.alpha = alpha;
            }

            if (introText != null)
            {
                introText.alpha = alpha;
            }

            yield return null;
        }

        if (introCanvasGroup != null)
        {
            introCanvasGroup.alpha = 0f;
            introCanvasGroup.interactable = false;
            introCanvasGroup.blocksRaycasts = false;
        }

        if (introText != null)
        {
            introText.gameObject.SetActive(false);
        }

        if (introPanel != null)
        {
            introPanel.SetActive(false);
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }

        Time.timeScale = 1f;
    }
}