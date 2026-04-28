using UnityEngine;
using UnityEngine.SceneManagement;

public class ShopEntrance : MonoBehaviour
{
    private bool playerNearby = false;

    void Update()
{
    if (playerNearby && Input.GetKeyDown(KeyCode.E))
    {
        WaveManager wm = FindFirstObjectByType<WaveManager>();

        if (wm != null)
            wm.SaveGameState();

        SceneManager.LoadScene("ItemShopScene");
    }
}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
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