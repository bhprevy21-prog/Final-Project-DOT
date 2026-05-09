using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    [Header("Hotbar")]
    public string[] hotbar = new string[4];

    [Header("Backpack")]
    public List<string> backpack = new List<string>();
    public int backpackLimit = 14;

    [Header("Selection")]
   public int selectedSlot = -1;
public string selectedItem = "";

    void Awake()
{
    Instance = this;

    hotbar = new string[4];
    backpack = new List<string>();
}

    void Start()
    {
        UpdateSelectedItem();
    }
    void Update()
{
    if (Input.GetKeyDown(KeyCode.Alpha1))
        SelectHotbarSlot(0);

    if (Input.GetKeyDown(KeyCode.Alpha2))
        SelectHotbarSlot(1);

    if (Input.GetKeyDown(KeyCode.Alpha3))
        SelectHotbarSlot(2);

    if (Input.GetKeyDown(KeyCode.Alpha4))
        SelectHotbarSlot(3);

    // remove selected item
    if (Input.GetKeyDown(KeyCode.R))
        RemoveSelectedHotbarItem();
}

    public void AddItem(string itemName)
    {
        // fill hotbar first
        for (int i = 0; i < hotbar.Length; i++)
        {
            if (string.IsNullOrEmpty(hotbar[i]))
            {
                hotbar[i] = itemName;

                if (selectedItem == "")
                {
                    selectedSlot = i;
                    UpdateSelectedItem();
                }

                return;
            }
        }

        // backpack overflow
        if (backpack.Count >= backpackLimit)
        {
            Debug.Log("Inventory Full");
            return;
        }

        backpack.Add(itemName);
    }

    public string GetHotbarItem(int slot)
    {
        if (slot < 0 || slot >= hotbar.Length)
            return "";

        return hotbar[slot];
    }

    public void SelectHotbarSlot(int index)
{
    if (index < 0 || index >= hotbar.Length)
        return;

    if (string.IsNullOrEmpty(hotbar[index]))
        return;

    selectedSlot = index;
    UpdateSelectedItem();

    Debug.Log("Selected: " + selectedItem);
}

    void UpdateSelectedItem()
{
    if (selectedSlot < 0 || selectedSlot >= hotbar.Length)
    {
        selectedItem = "";
        return;
    }

    selectedItem = hotbar[selectedSlot];
}

    public void SwapBackpackWithSelected(int backpackIndex)
    {
        if (backpackIndex < 0 || backpackIndex >= backpack.Count)
            return;

        string temp = hotbar[selectedSlot];
        hotbar[selectedSlot] = backpack[backpackIndex];
        backpack[backpackIndex] = temp;

        UpdateSelectedItem();
    }

    public void RemoveBackpackItem(int backpackIndex)
    {
        if (backpackIndex < 0 || backpackIndex >= backpack.Count)
            return;

        backpack.RemoveAt(backpackIndex);
    }
    public void RemoveSelectedHotbarItem()
{
    if (selectedSlot < 0)
        return;

    if (selectedSlot >= hotbar.Length)
        return;

    hotbar[selectedSlot] = "";

    selectedItem = "";
    selectedSlot = -1;

    Debug.Log("Removed item from hotbar");
}
public bool IsInventoryEmpty()
{
    // check hotbar
    for (int i = 0; i < hotbar.Length; i++)
    {
        if (!string.IsNullOrEmpty(hotbar[i]))
            return false;
    }

    // check backpack
    if (backpack.Count > 0)
        return false;

    return true;
}
}