using System;
using UnityEngine;

namespace RPG.Gameplay.Inventories
{
    public class InventorySlotUI : ItemSlotUI
    {
        private ItemSO _item;

        public event Action<ItemSO, GameObject> OnSlotSelected; // InventoryUI class has subscribed to this event

        /// <summary>
        /// Sets the item associated with this slot and updates the icon.
        /// </summary>
        /// <param name="item">The item to be associated with this slot.</param>
        public override void SetItem(ItemSO item)
        {
            _item = item;
            _icon.sprite = _item.Icon;
        }

        public override void ShowItemInfo() => OnSlotSelected?.Invoke(_item, gameObject);
    }
}
