using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public GameObject panel;

    [Header("Button Visual")]
    public Image buttonImage;
    public Sprite openSprite;
    public Sprite closeSprite;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    [Header("Backpack UI")]
    public Transform itemContainer;
    public GameObject slotPrefab;

    [Header("Info Panel")]
public GameObject itemInfoPanel;
public Image infoIcon;
public TMP_Text infoName;

private int selectedBackpackIndex = -1;

    [System.Serializable]
    public class ItemSprite
    {
        public string itemName;
        public Sprite sprite;
    }

    public ItemSprite[] itemSprites;

    private bool open = false;

    void Start()
    {
        panel.SetActive(false);
        buttonImage.sprite = closeSprite;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        open = !open;

        panel.SetActive(open);
        buttonImage.sprite = open ? openSprite : closeSprite;

        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);

        if (open)
            RefreshBackpackUI();
    }

  void RefreshBackpackUI()
{
    foreach (Transform child in itemContainer)
    {
        Destroy(child.gameObject);
    }

    for (int i = 0; i < PlayerInventory.Instance.backpack.Count; i++)
    {
        string item = PlayerInventory.Instance.backpack[i];

        GameObject slot = Instantiate(slotPrefab, itemContainer);

        InventorySlotVisual visual =
            slot.GetComponent<InventorySlotVisual>();

        if (visual != null)
            visual.SetIcon(GetSprite(item));

        InventorySlotButton button =
            slot.GetComponent<InventorySlotButton>();

        if (button != null)
        {
            button.itemName = item;
            button.backpackIndex = i;
        }
    }
}
    Sprite GetSprite(string itemName)
{
    foreach (var item in itemSprites)
    {
        if (item.itemName == itemName)
            return item.sprite;
    }

    Debug.LogWarning("Missing sprite for: " + itemName);
    return null;
}
public void ShowItemInfo(string itemName, int backpackIndex)
{
    selectedBackpackIndex = backpackIndex;

    itemInfoPanel.SetActive(true);

    infoName.text = itemName;
    infoIcon.sprite = GetSprite(itemName);

    RectTransform panelRect =
        itemInfoPanel.GetComponent<RectTransform>();

    if (Input.mousePosition.x < Screen.width / 2)
    {
        panelRect.anchoredPosition = new Vector2(250, 0);
    }
    else
    {
        panelRect.anchoredPosition = new Vector2(-250, 0);
    }
}

public void SwapSelected()
{
    PlayerInventory.Instance.SwapBackpackWithSelected(selectedBackpackIndex);
    RefreshBackpackUI();
}

public void RemoveSelected()
{
    PlayerInventory.Instance.RemoveBackpackItem(selectedBackpackIndex);
    itemInfoPanel.SetActive(false);
    RefreshBackpackUI();
}
}