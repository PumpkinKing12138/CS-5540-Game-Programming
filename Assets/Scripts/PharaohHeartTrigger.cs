using UnityEngine;

public class PharaohHeartTrigger : MonoBehaviour
{
    public LevelManager levelManager;

    [Header("Glass Requirement")]
    public GameObject[] glassSolidObjects;
    public float collectDelayAfterGlassBroken = 1f;

    [Header("Message Settings")]
    public float messageCooldown = 1f;

    private bool glassWasBroken = false;
    private float glassBrokenTime = -1f;
    private float nextMessageTime = 0f;

    void Update()
    {
        if (!glassWasBroken && IsAnyGlassBroken())
        {
            glassWasBroken = true;
            glassBrokenTime = Time.time;

            Debug.Log("Glass case was broken. Pharaoh's Heart can be touched after delay.");
        }
    }

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

        if (!glassWasBroken)
        {
            ShowMessageWithCooldown("glass");
            return;
        }

        float timeAfterGlassBroken = Time.time - glassBrokenTime;

        if (timeAfterGlassBroken < collectDelayAfterGlassBroken)
        {
            ShowMessageWithCooldown("delay");
            return;
        }

        Debug.Log("You touched Pharaoh's Heart.");

        levelManager.TryActivateHeart();

        if (levelManager.IsPortalOpened())
        {
            gameObject.SetActive(false);
        }
    }

    bool IsAnyGlassBroken()
    {
        if (glassSolidObjects == null || glassSolidObjects.Length == 0)
        {
            return false;
        }

        foreach (GameObject glassSolid in glassSolidObjects)
        {
            if (glassSolid == null)
            {
                continue;
            }

            if (!glassSolid.activeInHierarchy)
            {
                return true;
            }
        }

        return false;
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
        else if (messageType == "glass")
        {
            levelManager.ShowNeedGlassMessage();
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
