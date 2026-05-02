using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class LoadingManager : MonoBehaviour
{
    public static string sceneToLoad;

    [Header("UI")]
    public Image fadeImage;
    public TextMeshProUGUI tipText;
    public TextMeshProUGUI systemText;
    public Slider loadingBar;

    [Header("Spinner")]
    public RectTransform spinner;
    public float spinSpeed = 200f;

    [Header("Tips")]
    public string[] tips;

    [Header("System Messages")]
    public string[] systemMessages;

    [Header("Timing")]
    public float minLoadTime = 6f;
    public float maxLoadTime = 30f;

    float targetTime;
    float progress;

    void Start()
    {
        targetTime = Random.Range(minLoadTime, maxLoadTime);

        if (tips.Length > 0 && tipText != null)
            tipText.text = tips[Random.Range(0, tips.Length)];

        StartCoroutine(RandomSystemMessages());
        StartCoroutine(LoadRoutine());
    }

    void Update()
    {
        // spinner rotation
        if (spinner != null)
            spinner.Rotate(0f, 0f, -spinSpeed * Time.unscaledDeltaTime);

        // optional safety (kept from your original style)
        if (systemText == null) return;
    }

    IEnumerator RandomSystemMessages()
    {
        while (true)
        {
            if (systemMessages.Length > 0 && systemText != null)
            {
                systemText.text =
                    systemMessages[Random.Range(0, systemMessages.Length)];
            }

            yield return new WaitForSeconds(Random.Range(1.5f, 3.5f));
        }
    }

    IEnumerator LoadRoutine()
    {
        // start fully black → fade IN loading screen
        yield return StartCoroutine(Fade(1f, 0f, 0.5f));

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneToLoad);
        op.allowSceneActivation = false;

        float timer = 0f;

        while (!op.isDone)
        {
            timer += Time.unscaledDeltaTime;

            float targetProgress = Mathf.Clamp01(op.progress / 0.9f);
            progress = Mathf.MoveTowards(progress, targetProgress, Time.unscaledDeltaTime);

            if (loadingBar != null)
                loadingBar.value = progress;

            if (op.progress >= 0.9f && timer >= targetTime)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(0.25f);

        // fade BACK to black before switching scenes
        yield return StartCoroutine(Fade(0f, 1f, 0.5f));

        op.allowSceneActivation = true;
    }

    IEnumerator Fade(float from, float to, float duration)
    {
        float t = 0f;
        Color c = fadeImage.color;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;

            float a = Mathf.Lerp(from, to, t / duration);
            fadeImage.color = new Color(c.r, c.g, c.b, a);

            yield return null;
        }

        fadeImage.color = new Color(c.r, c.g, c.b, to);
    }
}