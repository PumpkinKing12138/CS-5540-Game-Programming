using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PharaohsHeartGame : MonoBehaviour
{
    public bool isGameFinished = false;
    public GameObject pauseMenuPanel;
    public GameObject winText;
    public Button[] cardButtons;
    public int[] cardNumbers;

    bool isGamePaused = false;
    private TextMeshProUGUI[] cardTexts;
    private bool[] isMatched;
    private bool[] isRevealed;

    private int firstCardIndex = -1;
    private bool isChecking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (isGamePaused)
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
        isGamePaused = false;
        Time.timeScale = 1.0f;
        pauseMenuPanel.SetActive(false);
    }

    public void PauseGame()
    {
        isGamePaused = true;
        Time.timeScale = 0f;
        pauseMenuPanel.SetActive(true);
        if (!isGameFinished)
        {
            winText.SetActive(false);
            GameStart();
        }
        else
        {
            winText.SetActive(true);
        }
    }

    void GameStart()
    {
        if (cardButtons.Length != cardNumbers.Length)
        {
            Debug.LogError("cardButtons 和 cardNumbers 的长度不一样！");
            return;
        }

        cardTexts = new TextMeshProUGUI[cardButtons.Length];
        isMatched = new bool[cardButtons.Length];
        isRevealed = new bool[cardButtons.Length];

        firstCardIndex = -1;
        isChecking = false;
        isGameFinished = false;

        for (int i = 0; i < cardButtons.Length; i++)
        {
            int index = i;

            cardButtons[i].gameObject.SetActive(true);
            cardButtons[i].interactable = true;

            cardTexts[i] = cardButtons[i].GetComponentInChildren<TextMeshProUGUI>();

            cardTexts[i].text = "";

            isMatched[i] = false;
            isRevealed[i] = false;

            cardButtons[i].onClick.RemoveAllListeners();
            cardButtons[i].onClick.AddListener(() => OnCardClicked(index));
        }
    }

    void OnCardClicked(int index)
    {
        if (isGameFinished) return;
        if (isChecking) return;
        if (isMatched[index]) return;
        if (isRevealed[index]) return;

        ShowCard(index);

        if (firstCardIndex == -1)
        {
            firstCardIndex = index;
        }
        else
        {
            int secondCardIndex = index;
            CheckTwoCards(firstCardIndex, secondCardIndex);
            firstCardIndex = -1;
        }
    }

    void ShowCard(int index)
    {
        isRevealed[index] = true;
        cardTexts[index].text = cardNumbers[index].ToString();
    }

    void HideCard(int index)
    {
        isRevealed[index] = false;
        cardTexts[index].text = "";
    }

    void CheckTwoCards(int firstIndex, int secondIndex)
    {
        if (cardNumbers[firstIndex] == cardNumbers[secondIndex])
        {
            isMatched[firstIndex] = true;
            isMatched[secondIndex] = true;

            cardButtons[firstIndex].gameObject.SetActive(false);
            cardButtons[secondIndex].gameObject.SetActive(false);

            CheckGameFinished();
        }
        else
        {
            StartCoroutine(HideWrongCards(firstIndex, secondIndex));
        }
    }

    IEnumerator HideWrongCards(int firstIndex, int secondIndex)
    {
        isChecking = true;

        // 因为你在 PauseGame 里设置了 Time.timeScale = 0
        // 所以这里必须用 WaitForSecondsRealtime
        yield return new WaitForSecondsRealtime(0.6f);

        HideCard(firstIndex);
        HideCard(secondIndex);

        isChecking = false;
    }

    void CheckGameFinished()
    {
        for (int i = 0; i < isMatched.Length; i++)
        {
            if (!isMatched[i])
            {
                return;
            }
        }

        isGameFinished = true;
        winText.SetActive(true);

        Debug.Log("Pharaoh's Heart Game Finished!");
    }
}
