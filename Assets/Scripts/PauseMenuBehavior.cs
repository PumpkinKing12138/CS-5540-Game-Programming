using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuBehavior : MonoBehaviour
{
    public GameObject pauseMenuPanel;
    bool isGamePause = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePause)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void ResumeGame()
    {
        isGamePause = false;
        Time.timeScale = 1.0f;
        pauseMenuPanel.SetActive(false);
    }

    public void PauseGame()
    {
        isGamePause = true;
        Time.timeScale = 0.0f;
        pauseMenuPanel.SetActive(true);
    }

    public void LoadMainMenu()
    {
        Debug.Log("Load Main Menu");
        SceneManager.LoadScene(0);
    }

    public void ExitGame()
    {
        Debug.Log("Exit game");
        Application.Quit();
    }

    public void StartGame()
    {
        Debug.Log("Start Game");
        SceneManager.LoadScene(1);
    }
}
