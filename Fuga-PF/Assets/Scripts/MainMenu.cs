using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Nome da primeira fase")]
    [SerializeField] private string firstLevelName = "Fase1";

    [Header("Painel de Controles")]
    [SerializeField] private GameObject controlsPanel;

    private void Start()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(firstLevelName);
    }

    public void OpenControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(true);
        }
    }

    public void CloseControls()
    {
        if (controlsPanel != null)
        {
            controlsPanel.SetActive(false);
        }
    }

    public void ExitGame()
    {
        Debug.Log("Botão Sair pressionado.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}