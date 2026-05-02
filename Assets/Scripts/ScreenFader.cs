using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    public static ScreenFader Instance;

    public Image fadeImage;
    public float fadeSpeed = 1.5f;

    void Awake()
    {
        // Singleton so it survives scene loads
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // When scene loads → fade FROM black
        FadeFromBlack();
    }

    public void FadeToBlack(System.Action onComplete = null)
    {
        StartCoroutine(Fade(1f, onComplete));
    }

    public void FadeFromBlack()
    {
        StartCoroutine(Fade(0f, null));
    }

    IEnumerator Fade(float targetAlpha, System.Action onComplete)
    {
        Color c = fadeImage.color;

        while (!Mathf.Approximately(c.a, targetAlpha))
        {
            c.a = Mathf.MoveTowards(c.a, targetAlpha, Time.unscaledDeltaTime * fadeSpeed);
            fadeImage.color = c;
            yield return null;
        }

        onComplete?.Invoke();
    }
}