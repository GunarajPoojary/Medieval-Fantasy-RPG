using UnityEngine;
using UnityEngine.EventSystems;

namespace RPG
{
    /// <summary>
    /// UI component for individual inventory slots, displaying items and handling interactions.
    /// </summary>
    public class InventorySlotUI : ItemSlotUI, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public virtual void OnPointerClick(PointerEventData eventData) => inventorySlotSelectedEvent.Invoke(this);

        public void OnPointerEnter(PointerEventData eventData)
        {
            //Debug.Log("Enter");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            //Debug.Log("Exit");
        }
    }
}