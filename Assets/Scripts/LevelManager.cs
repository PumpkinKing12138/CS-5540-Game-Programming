using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public int totalSeals = 3;

    public PharaohsHeartGame pharaohsHeartGame;
    public GameObject pharaohHeart;
    public GameObject portal;
    public GameMessageUI messageUI;

    [Header("Mummy Escape Phase")]
    public MummyEnemy[] mummies;
    public bool autoFindMummiesIfEmpty = true;

    [Header("BGM Settings")]
    public AudioSource bgmAudioSource;
    public AudioClip normalBGM;
    public AudioClip mummyTransformBGM;
    public AudioClip escapeBGM;
    public float mummyTransformBGMDuration = 3f;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    private int collectedSeals = 0;
    private bool heartUnlocked = false;
    private bool portalOpened = false;
    private bool isGameOver = false;
    private bool mummyEscapePhaseStarted = false;

    void Start()
    {
        Time.timeScale = 1f;

        isGameOver = false;
        collectedSeals = 0;
        heartUnlocked = false;
        portalOpened = false;
        mummyEscapePhaseStarted = false;

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

        PlayBGM(normalBGM, true);

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
        ShowMessage("All Seals collected! Break the glass case to reach Pharaoh's Heart.");
    }

    public bool IsHeartUnlocked()
    {
        return heartUnlocked;
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public bool IsPortalOpened()
    {
        return portalOpened;
    }

    public void ShowNeedSealsMessage()
    {
        ShowMessage("Pharaoh's Heart is locked. Collect all 3 Security Seals first.");
    }

    public void ShowNeedGlassMessage()
    {
        ShowMessage("Break the glass case first.");
    }

    public void ShowHeartDelayMessage()
    {
        ShowMessage("The glass just broke. Wait 1 second before touching Pharaoh's Heart.");
    }

    public void TryActivateHeart()
    {
        if (isGameOver)
        {
            return;
        }

        if (!heartUnlocked)
        {
            ShowNeedSealsMessage();
            return;
        }

        if (portalOpened)
        {
            ShowMessage("The portal is already open. Enter it to complete the level.");
            return;
        }

        if (pharaohsHeartGame != null)
        {
            if (!pharaohsHeartGame.isGameFinished)
            {
                pharaohsHeartGame.PauseGame();
                return;
            }
        }

        OpenPortal();
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

    public void StartMummyEscapePhase()
    {
        if (mummyEscapePhaseStarted)
        {
            return;
        }

        mummyEscapePhaseStarted = true;

        StartCoroutine(MummyEscapePhaseRoutine());
    }

    IEnumerator MummyEscapePhaseRoutine()
    {
        ShowMessage("Mummy is coming for you!");

        PlayBGM(mummyTransformBGM, true);

        if ((mummies == null || mummies.Length == 0) && autoFindMummiesIfEmpty)
        {
            mummies = FindObjectsOfType<MummyEnemy>();
        }

        if (mummies != null)
        {
            foreach (MummyEnemy mummy in mummies)
            {
                if (mummy != null)
                {
                    mummy.EnrageMummy();
                }
            }
        }

        yield return new WaitForSecondsRealtime(mummyTransformBGMDuration);

        PlayBGM(escapeBGM, true);
    }

    private void PlayBGM(AudioClip clip, bool loop)
    {
        if (bgmAudioSource == null || clip == null)
        {
            return;
        }

        bgmAudioSource.Stop();
        bgmAudioSource.clip = clip;
        bgmAudioSource.loop = loop;
        bgmAudioSource.Play();
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

        SceneManager.LoadScene(1);
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
