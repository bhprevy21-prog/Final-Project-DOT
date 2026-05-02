using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    public static ShopUI Instance;

    [Header("Panel")]
    public GameObject infoPanel;

    [Header("UI")]
    public Image itemIcon;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    public TMP_Text priceText;

    [Header("Purchase")]
    public Button purchaseButton;

    private string selectedItem = "";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        ShowWelcomeScreen();
    }

    public void ShowWelcomeScreen()
    {
        selectedItem = "";

        itemIcon.sprite = null;
        itemIcon.enabled = false;

        itemNameText.text = "Welcome to the Store";
        itemDescriptionText.text =
            "Here you can get some KILLER BATTLE CARDS to help you fight off those pesky pigeons that keep messing with the park.";

        priceText.text = "";

        purchaseButton.interactable = false;
    }

    public void ShowItem(string itemName)
    {
        selectedItem = itemName;

        ShopDatabase.ShopItem item =
            ShopDatabase.Instance.GetItem(itemName);

        if (item == null)
            return;

        itemIcon.enabled = true;
        itemIcon.sprite = item.icon;

        itemNameText.text = item.itemName;
        itemDescriptionText.text = item.description;
        priceText.text = "$" + item.price;

        purchaseButton.interactable = true;
    }

    public void Purchase()
{
    if (string.IsNullOrEmpty(selectedItem))
        return;

    ShopDatabase.ShopItem item =
        ShopDatabase.Instance.GetItem(selectedItem);

    if (item == null)
        return;

    WaveManager wm = WaveManager.Instance;

    if (wm == null)
    {
        Debug.LogError("WaveManager not found (ShopScene)");
        return;
    }

    if (wm.playerMoney < item.price)
    {
        Debug.Log("Not enough coins");
        return;
    }

    wm.AddMoney(-item.price);

    PlayerInventory.Instance.AddItem(selectedItem);

    Debug.Log("Bought: " + selectedItem);

    ShowWelcomeScreen();
}
}