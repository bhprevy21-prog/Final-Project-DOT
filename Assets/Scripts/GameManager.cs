using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;

    [Header("Other UI")]
public GameObject itemShopPanel;
public GameObject startButton;
public static bool InputLocked = false;
public CanvasGroup gameOverCanvasGroup;

    private bool isGameOver = false;

    void Awake()
    {
        // Singleton setup
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f; // Make sure game starts unpaused
    }

   public void StatueGameOver()
{
    if (isGameOver) return;

    isGameOver = true;

    Debug.Log("Game Over Triggered");

    InputLocked = true;

    if (itemShopPanel != null)
        itemShopPanel.SetActive(false);

    if (startButton != null)
        startButton.SetActive(false);

    gameOverPanel.SetActive(true);

    StartCoroutine(GameOverAnimation());

    Time.timeScale = 0f;
}

    // Restart button
    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Main menu button
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene"); // Make sure scene name matches exactly
    }
    IEnumerator GameOverAnimation()
{
    float duration = 0.5f;
    float time = 0;

    gameOverCanvasGroup.alpha = 0;

    // Optional: scale effect
    gameOverPanel.transform.localScale = Vector3.one * 1.2f;

    while (time < duration)
    {
        time += Time.unscaledDeltaTime;

        float t = time / duration;

        gameOverCanvasGroup.alpha = t;

        gameOverPanel.transform.localScale = Vector3.Lerp(
            Vector3.one * 1.2f,
            Vector3.one,
            t
        );

        yield return null;
    }

    gameOverCanvasGroup.alpha = 1;
}
}