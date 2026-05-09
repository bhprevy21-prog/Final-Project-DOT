using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public int playerMoney;

    [Header("Coin UI")]
    public TextMeshProUGUI hudCoinText;
    public TextMeshProUGUI shopCoinText;

    public void UpdateCoinsUI()
    {
        if (hudCoinText != null)
            hudCoinText.text = playerMoney.ToString();

        if (shopCoinText != null)
            shopCoinText.text = playerMoney.ToString();
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        UpdateCoinsUI();
    }
}