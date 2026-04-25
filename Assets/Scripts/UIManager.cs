using UnityEngine;
using TMPro;
using System.Collections;

public class CoinUI : MonoBehaviour
{
    public static CoinUI Instance;

    public TextMeshProUGUI coinText;

    private int currentDisplayedCoins = 0;
    private Coroutine countRoutine;

    void Awake()
    {
        Instance = this;
    }

    public void SetCoins(int targetAmount)
    {
        if (countRoutine != null)
            StopCoroutine(countRoutine);

        countRoutine = StartCoroutine(CountTo(targetAmount));
    }

    IEnumerator CountTo(int target)
    {
        while (currentDisplayedCoins != target)
        {
            currentDisplayedCoins = (int)Mathf.MoveTowards(currentDisplayedCoins, target, 5f);
            coinText.text = FormatCoins(currentDisplayedCoins);
            yield return null;
        }

        // ensure exact final value
        coinText.text = FormatCoins(target);
    }

    string FormatCoins(int amount)
    {
        if (amount >= 1000000)
            return (amount / 1000000f).ToString("0.#") + "M";

        if (amount >= 1000)
            return (amount / 1000f).ToString("0.#") + "K";

        return amount.ToString();
    }
}