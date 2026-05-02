using UnityEngine;

public class ShopDatabase : MonoBehaviour
{
    public static ShopDatabase Instance;

    [System.Serializable]
    public class ShopItem
    {
        public string itemName;
        public Sprite icon;
        public int price;
        [TextArea]
        public string description;
    }

    public ShopItem[] items;

    void Awake()
    {
        Instance = this;
    }

    public ShopItem GetItem(string name)
    {
        foreach (ShopItem item in items)
        {
            if (item.itemName == name)
                return item;
        }

        return null;
    }
}