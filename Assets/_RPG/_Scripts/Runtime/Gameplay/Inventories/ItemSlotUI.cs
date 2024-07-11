using UnityEngine;
using UnityEngine.UI;

namespace RPG.Gameplay.Inventories
{
    /// <summary>
    /// Represents a slot for holding items in the UI.
    /// </summary>
    public abstract class ItemSlotUI : MonoBehaviour
    {
        [SerializeField] protected Image _icon;

        [SerializeField] protected Button _button;

        /// <summary>
        /// Sets the item associated with this slot and updates the icon.
        /// </summary>
        /// <param name="item">The item to be associated with this slot.</param>
        public abstract void SetItem(ItemSO item);

        public abstract void ShowItemInfo();
    }
}
