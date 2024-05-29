
using UnityEngine;

namespace GunarajCode
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
            Inventories.Inventory.Instance.Remove(_item);

            _container.SetActive(false);

            Destroy(_slotPrefab);
        }

        public void UpgradeItem()
        {
            Debug.Log("Upgrade Mechanic has yet to come");
        }
    }
}
