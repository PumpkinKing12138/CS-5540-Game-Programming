using UnityEngine;
using TMPro;

public class PortalExit : MonoBehaviour
{
    public LevelManager levelManager;

    public TMP_Text messageText;

    public string completeMessage = "Congratulations! You escaped the Museum After Dark.";

    public bool pauseGameOnComplete = true;

    private bool gameCompleted = false;

    void Start()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (gameCompleted)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            if (levelManager != null && levelManager.IsPortalOpened())
            {
                gameCompleted = true;

                levelManager.CompleteLevel();

                if (messageText != null)
                {
                    messageText.text = completeMessage;
                    messageText.gameObject.SetActive(true);
                }

                if (pauseGameOnComplete)
                {
                    Time.timeScale = 0f;
                }
            }
        }
    }
}
