using UnityEngine;
using UnityEngine.SceneManagement;
public class MenuManager : MonoBehaviour
{
    public string startScene;

    public void LoadLevel()
    {
               SceneManager.LoadScene(startScene);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


}
