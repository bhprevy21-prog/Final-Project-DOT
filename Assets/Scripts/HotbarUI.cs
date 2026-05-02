using UnityEngine;
using UnityEngine.UI;

public class HotbarUI : MonoBehaviour
{
public Image[] slotIcons;  
public Image[] slotBackgrounds;       // item icon on top
    public Image[] slotHighlights;
    public Sprite emptySprite;

    [System.Serializable]
    public class ItemSprite
    {
        public string itemName;
        public Sprite sprite;
    }

    public ItemSprite[] itemSprites;

    void Update()
    {
        if (PlayerInventory.Instance == null)
            return;

        UpdateHotbarUI();
        UpdateHighlights();

        HandleDebugInput();
    }

void UpdateHotbarUI()
{
    if (PlayerInventory.Instance == null)
        return;

    if (slotIcons == null || slotIcons.Length == 0)
        return;

    for (int i = 0; i < slotIcons.Length; i++)
    {
        if (slotIcons[i] == null)
            continue;

        string item = PlayerInventory.Instance.GetHotbarItem(i);

        if (string.IsNullOrEmpty(item))
        {
            slotIcons[i].sprite = null;
            slotIcons[i].enabled = false;
        }
        else
        {
            slotIcons[i].enabled = true;
            slotIcons[i].sprite = GetSprite(item);
        }
    }
}
    void UpdateHighlights()
{
    int selected = PlayerInventory.Instance.selectedSlot;

    for (int i = 0; i < slotIcons.Length; i++)
    {
        if (slotIcons[i] == null)
            continue;

        if (i == selected)
        {
            // selected = normal / bright
            slotIcons[i].color = Color.white;
        }
        else
        {
            // unselected = greyed out
            slotIcons[i].color = new Color(0.6f, 0.6f, 0.6f, 1f);
        }
    }
}

    Sprite GetSprite(string itemName)
    {
        foreach (ItemSprite item in itemSprites)
        {
            if (item.itemName == itemName)
                return item.sprite;
        }

        return emptySprite;
    }

    void HandleDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            PlayerInventory.Instance.AddItem("Turret");

        if (Input.GetKeyDown(KeyCode.X))
            PlayerInventory.Instance.AddItem("ScarecrowSiren");

        if (Input.GetKeyDown(KeyCode.C))
            PlayerInventory.Instance.AddItem("BirdTrap");

        if (Input.GetKeyDown(KeyCode.V))
            PlayerInventory.Instance.AddItem("Coyote");

        if (Input.GetKeyDown(KeyCode.B))
            PlayerInventory.Instance.AddItem("ParkBoots");

        if (Input.GetKeyDown(KeyCode.N))
            PlayerInventory.Instance.AddItem("RoboCleaner");

        if (Input.GetKeyDown(KeyCode.M))
            PlayerInventory.Instance.AddItem("Teacher");

        if (Input.GetKeyDown(KeyCode.P))
            PlayerInventory.Instance.AddItem("Battery");
    }
}