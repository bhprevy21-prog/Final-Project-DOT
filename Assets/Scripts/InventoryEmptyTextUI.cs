using UnityEngine;
using UnityEngine.UI;

public class InventoryEmptyTextUI : MonoBehaviour
{
    public GameObject emptyText; // assign your "You don't have any items" UI text

    void Update()
    {
        if (PlayerInventory.Instance == null)
            return;

        if (PlayerInventory.Instance.IsInventoryEmpty())
        {
            emptyText.SetActive(true);
        }
        else
        {
            emptyText.SetActive(false);
        }
    }
}