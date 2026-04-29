using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopEntrance : MonoBehaviour
{
    private bool playerNearby = false;

    void Update()
    {
        WaveManager wm = FindFirstObjectByType<WaveManager>();

        // 🔒 BLOCK ENTRY DURING WAVE MODE
        if (wm != null && wm.currentMode == WaveManager.GameMode.Wave)
            return;

        if (playerNearby && Input.GetKeyDown(KeyCode.E))
        {
            if (wm != null)
                wm.SaveGameState();

            SceneManager.LoadScene("ItemShopScene");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            WaveManager wm = FindFirstObjectByType<WaveManager>();

            // 🔒 Prevent interaction in wave mode
            if (wm != null && wm.currentMode == WaveManager.GameMode.Wave)
                return;

            playerNearby = true;
            Debug.Log("Press E to enter shop");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;
        }
    }
}