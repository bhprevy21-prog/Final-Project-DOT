using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    public static PlayerInventory Instance;

    private Dictionary<string, int> items = new Dictionary<string, int>();

    void Awake()
    {
        Instance = this;
    }

    public void AddItem(string itemName)
    {
        if (!items.ContainsKey(itemName))
            items[itemName] = 0;

        items[itemName]++;
    }

    public int GetItemCount(string itemName)
    {
        if (!items.ContainsKey(itemName))
            return 0;

        return items[itemName];
    }
}