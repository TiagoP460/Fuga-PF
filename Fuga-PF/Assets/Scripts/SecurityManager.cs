using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SecurityManager : MonoBehaviour
{
    public static SecurityManager instance;

    [Header("Estado da Segurança")]
    public bool securityDisabled = false;

    public bool IsSecurityDisabled => securityDisabled;

    [Header("Alarme")]
    [SerializeField] private AudioSource alarmAudio;

    [Header("Tela Escura")]
    [SerializeField] private GameObject darkPanel;
    [SerializeField] private CanvasGroup darkCanvasGroup;
    [SerializeField] private float fadeDuration = 3f;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverCanvas;
    [SerializeField] private string firstSceneName = "Fase1";

    private bool gameOverStarted = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        securityDisabled = false;
        gameOverStarted = false;

        if (alarmAudio != null)
            alarmAudio.Stop();

        if (darkCanvasGroup != null)
            darkCanvasGroup.alpha = 0f;

        if (darkPanel != null)
            darkPanel.SetActive(false);

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(false);

        Time.timeScale = 1f;
    }

    public void DisableSecurity()
    {
        if (gameOverStarted)
            return;

        securityDisabled = true;

        Debug.Log("Sistema de segurança desativado.");
    }

    public void TriggerSecurityFail()
    {
        if (gameOverStarted)
            return;

        if (securityDisabled)
            return;

        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        gameOverStarted = true;

        if (alarmAudio != null)
            alarmAudio.Play();

        if (darkPanel != null)
            darkPanel.SetActive(true);

        if (darkCanvasGroup != null)
            darkCanvasGroup.alpha = 0f;

        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime;

            if (darkCanvasGroup != null)
            {
                darkCanvasGroup.alpha = Mathf.Lerp(
                    0f,
                    1f,
                    timer / fadeDuration
                );
            }

            yield return null;
        }

        if (darkCanvasGroup != null)
            darkCanvasGroup.alpha = 1f;

        if (gameOverCanvas != null)
            gameOverCanvas.SetActive(true);

        Time.timeScale = 0f;
    }

    public void RestartFromFirstPhase()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(firstSceneName);
    }
}