using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int totalSeals = 3;

    public GameObject pharaohHeart;
    public GameObject portal;
    public GameMessageUI messageUI;

    private int collectedSeals = 0;
    private bool heartUnlocked = false;
    private bool portalOpened = false;

    void Start()
    {
        if (portal != null)
        {
            portal.SetActive(false);
        }

        ShowMessage("Collect 3 Security Seals to unlock Pharaoh's Heart.");
    }

    public void CollectSeal()
    {
        collectedSeals++;

        ShowMessage("Security Seal collected: " + collectedSeals + " / " + totalSeals);

        if (collectedSeals >= totalSeals && !heartUnlocked)
        {
            UnlockPharaohHeart();
        }
    }

    private void UnlockPharaohHeart()
    {
        heartUnlocked = true;
        ShowMessage("All Seals collected! Pharaoh's Heart is now unlocked.");
    }

    public void TryActivateHeart()
    {
        if (!heartUnlocked)
        {
            ShowMessage("Pharaoh's Heart is locked. Collect all 3 Security Seals first.");
            return;
        }

        if (!portalOpened)
        {
            OpenPortal();
        }
        else
        {
            ShowMessage("The portal is already open. Enter it to complete the level.");
        }
    }

    private void OpenPortal()
    {
        portalOpened = true;

        if (portal != null)
        {
            portal.SetActive(true);
        }

        ShowMessage("Pharaoh's Heart activated! The exit portal is open.");
    }

    public bool IsPortalOpened()
    {
        return portalOpened;
    }

    public void PlayerCaughtByMummy()
    {
        ShowMessage("You were caught by the mummy!");
    }

    public void PlayerHitTrap()
    {
        ShowMessage("You stepped on a trap!");
    }

    public void CompleteLevel()
    {
        ShowMessage("Congratulations! You cleared the Egyptian Wing.");
    }

    private void ShowMessage(string message)
    {
        Debug.Log(message);

        if (messageUI != null)
        {
            messageUI.ShowMessage(message);
        }
    }
}
