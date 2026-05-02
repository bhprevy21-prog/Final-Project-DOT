using UnityEngine;

public class HidePanelButton : MonoBehaviour
{
    public GameObject panelToHide;

    public void HidePanel()
    {
        panelToHide.SetActive(false);
    }
}