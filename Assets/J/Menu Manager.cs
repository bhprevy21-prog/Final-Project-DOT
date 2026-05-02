using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public string startScene = "WaveScene";

    public void LoadLevel()
    {
        LoadingManager.sceneToLoad = startScene;
        SceneManager.LoadScene("LoadingScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}