using UnityEngine;

public class ShopButton : MonoBehaviour
{
    public string itemName;

    public void Click()
    {
        ShopUI.Instance.ShowItem(itemName);
    }
}