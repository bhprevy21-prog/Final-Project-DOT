using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    [Header("Pause UI")]
public GameObject pausePanel;
public CanvasGroup pauseCanvasGroup;

private bool isPaused = false;
[Header("Pause Tips")]
public TMPro.TextMeshProUGUI tipText;

[System.Serializable]
public class PauseTip
{
    [TextArea(2,4)]
    public string text;
}

public PauseTip[] pauseTips;
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

if (pausePanel != null)
    pausePanel.SetActive(false);

Time.timeScale = 1f;
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
public void PauseGame()
{
    if (isPaused) return;

    isPaused = true;
    InputLocked = true;

    if (itemShopPanel != null)
        itemShopPanel.SetActive(false);

   ShowRandomPauseTip();
pausePanel.SetActive(true);

    StartCoroutine(PauseAnimation());

    Time.timeScale = 0f;
}
public void ResumeGame()
{
    if (!isPaused) return;

    isPaused = false;
    InputLocked = false;

    pausePanel.SetActive(false);

    Time.timeScale = 1f;
}
void ShowRandomPauseTip()
{
    if (pauseTips == null || pauseTips.Length == 0)
        return;

    int index = Random.Range(0, pauseTips.Length);

    if (tipText != null)
        tipText.text = pauseTips[index].text;
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
        SceneManager.LoadScene("MainMenu"); // Make sure scene name matches exactly
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
void Update()
{
    if (isGameOver)
        return;

    if (Input.GetKeyDown(KeyCode.Escape))
    {
        if (isPaused)
            ResumeGame();
        else
            PauseGame();
    }
}
IEnumerator PauseAnimation()
{
    float duration = 0.35f;
    float time = 0;

    pauseCanvasGroup.alpha = 0;
    pausePanel.transform.localScale = Vector3.one * 1.15f;

    while (time < duration)
    {
        time += Time.unscaledDeltaTime;

        float t = time / duration;

        pauseCanvasGroup.alpha = t;

        pausePanel.transform.localScale =
            Vector3.Lerp(
                Vector3.one * 1.15f,
                Vector3.one,
                t
            );

        yield return null;
    }

    pauseCanvasGroup.alpha = 1;
}
}