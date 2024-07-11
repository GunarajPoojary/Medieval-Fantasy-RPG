using UnityEngine;

namespace RPG.Gameplay.Inventories
{
    /// <summary>
    /// This class handles displaying information about an inventory item in the UI.
    /// It updates the UI elements with the item's details and provides functionality
    /// to sell the item or upgrade it.
    /// </summary>
    public class InventoryItemInfo : ItemInfoUI
    {
        public void SellItem()
        {
            Inventory.Instance.RemoveItem(_item);

            _container.SetActive(false);

            Destroy(_slotPrefab);
        }

        public void EnhanceItem()
        {
            Debug.Log("Upgrade Mechanic has yet to come");
        }

        /// <summary>
        /// Uses the item associated with this slot and destroys the slot object.
        /// </summary>
        public void UseItem()
        {
            _item.Use();
            Destroy(gameObject);
        }
    }
}
