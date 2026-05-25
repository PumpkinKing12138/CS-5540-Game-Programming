using UnityEngine;
using TMPro;
using System.Collections;

public class GameMessageUI : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float displayTime = 3f;

    private Coroutine currentMessage;

    void Start()
    {
        ClearMessage();
    }

    public void ShowMessage(string message)
    {
        if (currentMessage != null)
        {
            StopCoroutine(currentMessage);
        }

        currentMessage = StartCoroutine(ShowMessageRoutine(message));
    }

    private IEnumerator ShowMessageRoutine(string message)
    {
        messageText.text = message;
        yield return new WaitForSeconds(displayTime);
        ClearMessage();
    }

    public void ClearMessage()
    {
        if (messageText != null)
        {
            messageText.text = "";
        }
    }
}
