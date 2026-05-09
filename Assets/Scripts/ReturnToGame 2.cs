using UnityEngine;

public class ReturnToGame : MonoBehaviour
{
    public GameObject shopPanel;

    public void Return()
    {
        shopPanel.SetActive(false);
        Debug.Log("Closed Shop");
    }
}