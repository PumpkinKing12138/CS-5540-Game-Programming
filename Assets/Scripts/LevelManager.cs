using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int totalSeals = 3;

    public PharaohsHeartGame pharaohsHeartGame;
    public GameObject pharaohHeart;
    public GameObject portal;
    public GameMessageUI messageUI;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    private int collectedSeals = 0;
    private bool heartUnlocked = false;
    private bool portalOpened = false;
    private bool isGameOver = false;

    void Start()
    {
        Time.timeScale = 1f;

        isGameOver = false;
        collectedSeals = 0;
        heartUnlocked = false;
        portalOpened = false;

        if (portal != null)
        {
            portal.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (messageUI != null)
        {
            messageUI.gameObject.SetActive(true);
            messageUI.ClearMessage();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ShowMessage("Collect 3 Security Seals to unlock Pharaoh's Heart.");
    }

    public void CollectSeal()
    {
        if (isGameOver)
        {
            return;
        }

        collectedSeals++;

        ShowMessage("Security Seal collected: " + collectedSeals + " / " + totalSeals);

        if (collectedSeals >= totalSeals && !heartUnlocked)
        {
            UnlockPharaohHeart();
        }
    }

    private void UnlockPharaohHeart()
    {
        if (isGameOver)
        {
            return;
        }

        heartUnlocked = true;
        ShowMessage("All Seals collected! Pharaoh's Heart is now unlocked.");
    }

    public void TryActivateHeart()
    {
        if (isGameOver)
        {
            return;
        }

        if (!heartUnlocked)
        {
            ShowMessage("Pharaoh's Heart is locked. Collect all 3 Security Seals first.");
            return;
        }

        if (!pharaohsHeartGame.isGameFinished)
            pharaohsHeartGame.PauseGame();

        if (!portalOpened)
        {
            if (pharaohsHeartGame.isGameFinished)
                OpenPortal();
        }
        else
        {
            ShowMessage("The portal is already open. Enter it to complete the level.");
        }
    }

    private void OpenPortal()
    {
        if (isGameOver)
        {
            return;
        }

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
        if (isGameOver)
        {
            return;
        }

        ShowMessage("You were caught by the mummy!");
    }

    public void PlayerHitTrap()
    {
        if (isGameOver)
        {
            return;
        }

        ShowMessage("You stepped on a trap!");
    }

    public void GameOver()
    {
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;

        Debug.Log("Game Over!");

        if (messageUI != null)
        {
            messageUI.ClearMessage();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            gameOverPanel.transform.SetAsLastSibling();
        }

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void StartOver()
    {
        Debug.Log("Start Over button clicked.");

        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void CompleteLevel()
    {
        if (isGameOver)
        {
            return;
        }

        ShowMessage("Congratulations! You cleared the Egyptian Wing.");
    }

    private void ShowMessage(string message)
    {
        if (isGameOver)
        {
            return;
        }

        Debug.Log(message);

        if (messageUI != null)
        {
            messageUI.ShowMessage(message);
        }
    }
}
