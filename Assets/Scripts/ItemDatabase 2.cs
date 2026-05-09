using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public GameObject prefab;
    public Sprite icon;
}

public class ItemDatabase : MonoBehaviour
{
    public static ItemDatabase Instance;

    public ItemData[] items;

    void Awake()
    {
        Instance = this;
    }

    public GameObject GetPrefab(string itemName)
    {
        foreach (var item in items)
        {
            if (item.itemName == itemName)
                return item.prefab;
        }

        return null;
    }

    public Sprite GetIcon(string itemName)
    {
        foreach (var item in items)
        {
            if (item.itemName == itemName)
                return item.icon;
        }

        return null;
    }
}