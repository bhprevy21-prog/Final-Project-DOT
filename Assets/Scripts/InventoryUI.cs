using UnityEngine;
using UnityEngine.UI;

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
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}