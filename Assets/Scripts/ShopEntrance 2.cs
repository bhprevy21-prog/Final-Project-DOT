using UnityEngine;

public class ShopEntrance : MonoBehaviour
{
    public GameObject shopPanel;

    private bool playerNearby = false;

    void Update()
    {
        WaveManager wm = FindFirstObjectByType<WaveManager>();

        // block shop during waves
        if (wm != null && wm.currentMode == WaveManager.GameMode.Wave)
            return;

        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            shopPanel.SetActive(true);
            Debug.Log("Opened Shop");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        WaveManager wm = FindFirstObjectByType<WaveManager>();

        if (wm != null && wm.currentMode == WaveManager.GameMode.Wave)
            return;

        playerNearby = true;
        Debug.Log("Press E to enter shop");
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerNearby = false;
    }
}