using AYellowpaper.SerializedCollections;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.Inventories.UI
{
    /// <summary>
    /// Manages the inventory slots, placing items in the correct UI panels based on their type.
    /// </summary>
    public class InventorySlotsInstantiator : MonoBehaviour, IInventorySlotInstantiator
    {
        [SerializedDictionary("Item Type", "Item Panel")]
        [SerializeField] private SerializedDictionary<ItemType, Transform> _itemTypeToPanelsMap;

        [SerializeField] private GameObject _inventorySlotPrefab;

        private void OnValidate()
        {
            if (_itemTypeToPanelsMap.ContainsKey(ItemType.Default))
            {
                _itemTypeToPanelsMap.Remove(ItemType.Default);
            }
        }

        public void AddItemToUI(ItemSO item)
        {
            if (_itemTypeToPanelsMap.TryGetValue(item.Type, out Transform container))
            {
                Instantiate(_inventorySlotPrefab, container).GetComponent<InventorySlotUI>().SetItem(item);
            }
        }
    }
}