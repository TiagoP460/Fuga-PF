using UnityEngine;
using TMPro;
using System.Collections;

public class MessageManager : MonoBehaviour
{
    public static MessageManager instance;

    public TextMeshProUGUI messageText;
    public float messageDuration = 2f;

    private Coroutine currentMessage;

    void Awake()
    {
        instance = this;
    }

    public void ShowMessage(string message)
    {
        if (currentMessage != null)
        {
            StopCoroutine(currentMessage);
        }

        currentMessage = StartCoroutine(ShowMessageRoutine(message));
    }

    IEnumerator ShowMessageRoutine(string message)
    {
        messageText.text = message;

        yield return new WaitForSeconds(messageDuration);

        messageText.text = "";
    }
}