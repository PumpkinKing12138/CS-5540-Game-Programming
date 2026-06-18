using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;
    public int startingMoney = 2;
    public TMP_Text moneyText;
    int currentMoney;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentMoney = startingMoney;
        UpdateMoneyText();
    }

    public bool BuyTower(int cost)
    {
        if (currentMoney >= cost)
        {
            currentMoney -= cost;
            UpdateMoneyText();
            return true;
        }
        return false;
    }

    public void GetMoney(int reward)
    {
        currentMoney += reward;
        UpdateMoneyText();
    }

    public void UpdateMoneyText()
    {
        if (moneyText)
        {
            moneyText.text = currentMoney.ToString();
        }
    }

    public int GetCurrentMoney()
    {
        return currentMoney;
    }
}
