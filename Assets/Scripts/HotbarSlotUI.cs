using UnityEngine;
using UnityEngine.EventSystems;

public class HotbarSlotUI : MonoBehaviour, IPointerClickHandler
{
    public int slotIndex;

    public void OnPointerClick(PointerEventData eventData)
    {
        PlayerInventory.Instance.SelectHotbarSlot(slotIndex);
    }
}