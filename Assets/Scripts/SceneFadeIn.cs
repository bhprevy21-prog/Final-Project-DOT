using UnityEngine;
using System.Collections;

public class SceneFadeIn : MonoBehaviour
{
    public CanvasGroup fadeGroup;
    public float fadeTime = 0.5f;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        // ✅ safety: make sure object is visible
        if (fadeGroup != null)
            fadeGroup.gameObject.SetActive(true);

        // ✅ force full black instantly (alpha 1 = 255 equivalent)
        fadeGroup.alpha = 1f;

        float t = 0f;

        // fade to transparent
        while (t < fadeTime)
        {
            t += Time.unscaledDeltaTime;
            fadeGroup.alpha = 1f - (t / fadeTime);
            yield return null;
        }

        fadeGroup.alpha = 0f;

        // optional: disable after fade for cleanliness
        fadeGroup.gameObject.SetActive(false);
    }
}