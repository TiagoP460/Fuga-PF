using UnityEngine;
using TMPro;
using System.Collections;

public class MessageManager : MonoBehaviour
{
    public static MessageManager instance;

    [Header("Texto da Mensagem")]
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Configuração")]
    [SerializeField] private float messageDuration = 2f;

    private Coroutine currentMessage;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        HideMessage();
    }

    public void ShowMessage(string message)
    {
        if (messageText == null)
        {
            Debug.LogWarning("MessageText não foi colocado no MessageManager.");
            return;
        }

        if (currentMessage != null)
        {
            StopCoroutine(currentMessage);
            currentMessage = null;
        }

        if (string.IsNullOrEmpty(message))
        {
            HideMessage();
            return;
        }

        currentMessage = StartCoroutine(ShowMessageRoutine(message));
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        messageText.gameObject.SetActive(true);
        messageText.text = message;

        yield return new WaitForSeconds(messageDuration);

        HideMessage();
    }

    public void HideMessage()
    {
        if (messageText == null)
            return;

        messageText.text = "";
        messageText.gameObject.SetActive(false);
    }
}