using UnityEngine;
using TMPro;

public class DoorBehavior : MonoBehaviour
{
    public TMP_Text messageText;
    public string completeMessage = "Congratulations! You escaped the Museum After Dark.";
    public int levelfinished = 0;
    public int ID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelfinished = PlayerPrefs.GetInt("levelfinished"+ID, 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        if (levelfinished == 1)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log(PlayerPrefs.GetInt("levelfinished" + 1, 0));
            Debug.Log(PlayerPrefs.GetInt("levelfinished" + 2, 0));
            if (ID == 3 && PlayerPrefs.GetInt("levelfinished" + 1, 0) == 1 && PlayerPrefs.GetInt("levelfinished" + 2, 0) == 1)
            {
                Time.timeScale = 0f;
                if (messageText != null)
                {
                    messageText.text = completeMessage;
                    messageText.gameObject.SetActive(true);
                }
            }
            levelfinished = 1;
            PlayerPrefs.SetInt("levelfinished"+ID, levelfinished);
            CenterLevelManager.ManageLevel(ID);
        }
    }
}
