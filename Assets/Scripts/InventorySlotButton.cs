using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventorySlotButton : MonoBehaviour
{
    public string itemName;
    public int backpackIndex;

    public void Click()
    {
        InventoryUI ui = FindFirstObjectByType<InventoryUI>();
        ui.ShowItemInfo(itemName, backpackIndex);
    }
}