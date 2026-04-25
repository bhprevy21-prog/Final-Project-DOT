using UnityEngine;
using System.Collections;

public class ShopToggle : MonoBehaviour
{
    public RectTransform shopPanel;

    public Vector2 openPosition;
    public Vector2 closedPosition;

    public float slideSpeed = 10f;

    private bool isOpen = true;
    private Coroutine slideRoutine;

    public void ToggleShop()
    {
        transform.Rotate(0, 0, 180f);
        isOpen = !isOpen;

        if (slideRoutine != null)
            StopCoroutine(slideRoutine);

        if (isOpen)
            slideRoutine = StartCoroutine(SlideTo(openPosition));
        else
            slideRoutine = StartCoroutine(SlideTo(closedPosition));
    }

    IEnumerator SlideTo(Vector2 target)
    {
        while (Vector2.Distance(shopPanel.anchoredPosition, target) > 0.1f)
        {
            shopPanel.anchoredPosition = Vector2.Lerp(
                shopPanel.anchoredPosition,
                target,
                Time.deltaTime * slideSpeed
            );

            yield return null;
        }

        shopPanel.anchoredPosition = target;
    }
}