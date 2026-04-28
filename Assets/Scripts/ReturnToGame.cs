using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToGame : MonoBehaviour
{
    public void Return()
    {
        SceneManager.LoadScene("WaveScene");
    }
}