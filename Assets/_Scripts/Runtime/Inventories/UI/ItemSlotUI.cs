using RPG.ScriptableObjects.Items;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG.Inventories.UI
{
    /// <summary>
    /// Abstract base class for item slot UI components, handling icon updates and interaction events.
    /// </summary>
    public abstract class ItemSlotUI : MonoBehaviour, IItemSetter, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] protected Image _icon;
        [SerializeField] protected Button _button;

        #region IItemSetter Method
        public virtual void SetItem(ItemSO item) => UpdateIcon();
        #endregion

        public abstract void DisplayItemOverview();

        protected abstract void UpdateIcon();

        public void OnPointerClick(PointerEventData eventData)
        {
            DisplayItemOverview();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }
    }
}