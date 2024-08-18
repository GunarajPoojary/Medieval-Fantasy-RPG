using RPG.ScriptableObjects.Items;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Inventories.UI
{
    /// <summary>
    /// Abstract base class for item slot UI components, handling icon updates and interaction events.
    /// </summary>
    public abstract class ItemSlotUI : MonoBehaviour, IItemSetteable
    {
        [SerializeField] protected Image _icon;
        [SerializeField] protected Button _button;

        private void OnEnable() => _button?.onClick.AddListener(DisplayItemOverview);

        private void OnDisable() => _button?.onClick.RemoveListener(DisplayItemOverview);

        #region IItemSetter Method
        public virtual void SetItem(ItemSO item) => UpdateIcon();
        #endregion

        public abstract void DisplayItemOverview();

        protected abstract void UpdateIcon();
    }
}