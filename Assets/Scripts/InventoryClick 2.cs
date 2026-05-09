using UnityEngine;

public class InventoryClick : MonoBehaviour
{
    public InventoryUI ui;

    void OnMouseDown()
    {
        ui.ToggleInventory();
    }
}