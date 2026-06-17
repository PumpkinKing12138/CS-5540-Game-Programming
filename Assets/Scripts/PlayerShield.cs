using UnityEngine;
using UnityEngine.UI;

public class PlayerShield : MonoBehaviour
{
    [Header("Shield Settings")]
    public int shieldCount = 0;
    public int maxShieldCount = 3;

    [Header("UI")]
    public Image[] shieldIcons;

    void Start()
    {
        UpdateShieldUI();
    }

    public void AddShield()
    {
        shieldCount++;

        if (shieldCount > maxShieldCount)
        {
            shieldCount = maxShieldCount;
        }

        UpdateShieldUI();
    }

    public bool UseShield()
    {
        if (shieldCount <= 0)
        {
            return false;
        }

        shieldCount--;

        if (shieldCount < 0)
        {
            shieldCount = 0;
        }

        UpdateShieldUI();

        return true;
    }

    void UpdateShieldUI()
    {
        if (shieldIcons == null)
        {
            return;
        }

        for (int i = 0; i < shieldIcons.Length; i++)
        {
            if (shieldIcons[i] == null)
            {
                continue;
            }

            shieldIcons[i].gameObject.SetActive(i < shieldCount);
        }
    }
}
