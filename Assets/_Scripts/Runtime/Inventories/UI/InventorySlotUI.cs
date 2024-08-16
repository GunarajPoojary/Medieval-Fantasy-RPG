using RPG.ScriptableObjects.EventChannels;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.Inventories.UI
{
    /// <summary>
    /// UI component for individual inventory slots, displaying items and handling interactions.
    /// </summary>
    public class InventorySlotUI : ItemSlotUI
    {
        [SerializeField] private VoidReturnItemSOAndGameObjectParameterEventChannelSO _inventorySlotSelectionEventChannelSO;  // Event channel for slot selection

        private ItemSO _item;

        /// <summary>
        /// Sets the Item for this slot and updates the icon.
        /// </summary>
        public override void SetItem(ItemSO item)
        {
            _item = item;
            base.SetItem(item);
        }

        /// <summary>
        /// Displays the Item overview when the slot is clicked, raising the event.
        /// </summary>
        public override void DisplayItemOverview() => _inventorySlotSelectionEventChannelSO.RaiseEvent(_item, gameObject);

        protected override void UpdateIcon() => _icon.sprite = _item.Icon;
    }
}