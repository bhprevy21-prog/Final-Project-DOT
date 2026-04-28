using UnityEngine;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public string itemName = "Turret";

    public TextMeshProUGUI countText;

    void Update()
    {
        int count = PlayerInventory.Instance.GetItemCount(itemName);
        countText.text = count.ToString();
    }
}