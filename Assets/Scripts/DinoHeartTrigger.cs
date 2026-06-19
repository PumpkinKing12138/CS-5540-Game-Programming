using UnityEngine;

public class DinoHeartTrigger : MonoBehaviour
{
    public LevelManager levelManager;
    public bool getKey = false;

    [Header("Message Settings")]
    public float messageCooldown = 1f;

    private float nextMessageTime = 0f;

    void OnTriggerEnter(Collider other)
    {
        CheckHeartTouch(other);
    }

    void OnTriggerStay(Collider other)
    {
        CheckHeartTouch(other);
    }

    void CheckHeartTouch(Collider other)
    {
        if (!IsPlayer(other))
        {
            return;
        }

        if (levelManager == null)
        {
            Debug.Log("Level Manager is not assigned on PharaohHeartTrigger.");
            return;
        }

        if (levelManager.IsGameOver())
        {
            return;
        }

        if (!levelManager.IsHeartUnlocked())
        {
            ShowMessageWithCooldown("seals");
            return;
        }

        if (!getKey)
        {
            ShowMessageWithCooldown("key");
            return;
        }

        Debug.Log("You touched Pharaoh's Heart.");

        levelManager.TryActivateHeart();

        if (levelManager.IsPortalOpened())
        {
            gameObject.SetActive(false);
        }
    }

    void ShowMessageWithCooldown(string messageType)
    {
        if (Time.time < nextMessageTime)
        {
            return;
        }

        nextMessageTime = Time.time + messageCooldown;

        if (levelManager == null)
        {
            return;
        }

        if (messageType == "seals")
        {
            levelManager.ShowNeedSealsMessage();
        }
        else if (messageType == "key")
        {
            levelManager.ShowNeedKeyMessage();
        }
        else if (messageType == "delay")
        {
            levelManager.ShowHeartDelayMessage();
        }
    }

    bool IsPlayer(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            return true;
        }

        if (other.transform.root.CompareTag("Player"))
        {
            return true;
        }

        return false;
    }
}
