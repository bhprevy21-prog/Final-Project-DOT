using UnityEngine;
using UnityEngine.UI;

public class InventorySlotVisual : MonoBehaviour
{
    public Image iconImage;

   public void SetIcon(Sprite sprite)
{
    if (iconImage == null)
        return;

    iconImage.sprite = sprite;

    // IMPORTANT: never disable image
    iconImage.color = Color.white;

    // only hide sprite visually if missing
    if (sprite == null)
    {
        iconImage.color = new Color(1,1,1,0); // transparent
    }
    else
    {
        iconImage.color = Color.white;
    }
}
}