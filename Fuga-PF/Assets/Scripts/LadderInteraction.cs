using UnityEngine;
using UnityEngine.SceneManagement;

public class LadderInteraction : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private SecurityManager securityManager;

    [Header("Cena")]
    [SerializeField] private string nextSceneName = "Fase4";

    private bool playerNear = false;

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

    private void Update()
    {
        if (!playerNear)
            return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (securityManager != null && securityManager.IsSecurityDisabled)
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                ShowMessage("Desative o sistema de segurança para prosseguir.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerNear = true;

        ShowMessage("Aperte E para descer.");
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player"))
            return;

        playerNear = false;
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