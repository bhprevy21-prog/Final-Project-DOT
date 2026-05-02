using UnityEngine;

public class SceneStartFade : MonoBehaviour
{
    void Start()
    {
        if (ScreenFader.Instance != null)
        {
            ScreenFader.Instance.FadeFromBlack();
        }
    }
}