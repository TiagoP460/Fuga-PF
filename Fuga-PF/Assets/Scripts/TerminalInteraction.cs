using UnityEngine;

public class TerminalInteraction : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private SecurityManager securityManager;
    [SerializeField] private GameObject minigameCanvas;
    [SerializeField] private WireMinigameManager wireMinigameManager;

    [Header("Player")]
    [SerializeField] private Behaviour playerMovementScript;

    private bool playerNear = false;
    private bool minigameOpen = false;

    private void Awake()
    {
        if (securityManager == null)
        {
            securityManager = SecurityManager.instance;
        }

        if (securityManager == null)
        {
            securityManager = FindAnyObjectByType<SecurityManager>();
        }
    }

    private void Start()
    {
        if (minigameCanvas != null)
        {
            minigameCanvas.SetActive(false);
        }
    }

    private void Update()
    {
        if (!playerNear)
            return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (securityManager != null && securityManager.IsSecurityDisabled)
            {
                ShowMessage("O sistema de segurança já está desativado.");
                return;
            }

            OpenMinigame();
        }

        if (minigameOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseMinigame();
            ShowMessage("Aperte F para desativar o sistema de segurança.");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerNear = true;

        if (securityManager != null && securityManager.IsSecurityDisabled)
        {
            ShowMessage("Sistema de segurança já desativado.");
        }
        else
        {
            ShowMessage("Aperte F para desativar o sistema de segurança.");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerNear = false;

        if (!minigameOpen)
        {
            ShowMessage("");
        }
    }

    private void OpenMinigame()
    {
        minigameOpen = true;

        ShowMessage("");

        if (minigameCanvas != null)
        {
            minigameCanvas.SetActive(true);
        }

        if (wireMinigameManager != null)
        {
            wireMinigameManager.ResetMinigame();
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = false;
        }
    }

    private void CloseMinigame()
    {
        minigameOpen = false;

        if (minigameCanvas != null)
        {
            minigameCanvas.SetActive(false);
        }

        if (playerMovementScript != null)
        {
            playerMovementScript.enabled = true;
        }
    }

    public void ResolveWireMinigame(bool success)
    {
        CloseMinigame();

        if (success)
        {
            if (securityManager != null)
            {
                securityManager.DisableSecurity();
            }

            ShowMessage("Sistema de segurança desativado. Volte para a escada.");
        }
        else
        {
            ShowMessage("");

            if (securityManager != null)
            {
                securityManager.TriggerSecurityFail();
            }
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