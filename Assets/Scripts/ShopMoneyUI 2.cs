using UnityEngine;
using TMPro;

public class ShopCoinUI : MonoBehaviour
{
    public TextMeshProUGUI text;

    void Update()
    {
        WaveManager wm = WaveManager.Instance;

        if (wm == null)
            return;

        text.text = "Coins: " + wm.playerMoney;
    }
}