using RPG.Core.SaveLoad;
using RPG.Core.Utils;
using RPG.ScriptableObjects.EventChannels;
using RPG.ScriptableObjects.Items;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    public class Inventory : SimpleSingleton<Inventory>, ISaveable
    {
        [SerializeField] private int _maxCapacity = 2000;
        [Space]
        [SerializeField] private ItemSOReturnStringParameterEventChannelSO _itemLookupEvent;  // Event channel for item lookup
        [Space]
        [SerializeField] private VoidReturnItemSOParameterEventChannelSO _addItem;  // Event channel to add items
        [Space]
        [SerializeField] private VoidReturnItemSOParameterEventChannelSO _removeItem;  // Event channel to remove items
        [Space]
        [SerializeField] private VoidReturnItemSOParameterEventChannelSO _itemAdded;  // Event channel triggered when an item is added
        [Space]
        [SerializeField] private VoidReturnItemSOParameterEventChannelSO _itemRemoved;  // Event channel triggered when an item is removed
        [Space]
        [SerializeField] private ItemSOListReturnNonParameterEventChannelSO _getItems;  // Event channel to get the list of items

        private List<ItemSO> _items = new List<ItemSO>();

        private void OnEnable()
        {
            _addItem.OnEventRaised += AddItem;
            _removeItem.OnEventRaised += RemoveItem;
            _getItems.OnEventRaised += GetItems;
        }

        private void OnDestroy()
        {
            _addItem.OnEventRaised -= AddItem;
            _removeItem.OnEventRaised -= RemoveItem;
            _getItems.OnEventRaised -= GetItems;
        }

        public List<ItemSO> GetItems() => _items;

        public void AddItem(ItemSO item)
        {
            if (_items.Count >= _maxCapacity)
            {
                Debug.LogError("Inventory is full. Cannot add more items.");
                return;
            }

            if (item == null)
            {
                Debug.LogError("Cannot add null item.");
                return;
            }

            if (_items.Contains(item))
            {
                Debug.LogError("Duplicate item detected in the inventory.");
                return;
            }

            _items.Add(item);
            _itemAdded.RaiseEvent(item);
        }

        public void RemoveItem(ItemSO item)
        {
            if (item == null)
            {
                Debug.LogError("Cannot remove null item.");
                return;
            }

            if (_items.Remove(item))
            {
                _itemRemoved.RaiseEvent(item);
            }
            else
            {
                Debug.LogError("Item not found in the inventory.");
            }
        }

        #region ISaveable Methods
        public void LoadData(GameData data)
        {
            _items = new List<ItemSO>();

            foreach (string itemID in data.ItemIDs)
            {
                ItemSO item = _itemLookupEvent.RaiseEvent(itemID);

                if (item != null)
                {
                    _items.Add(item);
                }
                else
                {
                    Debug.LogWarning($"Item with ID {itemID} not found in the database.");
                }
            }
        }

        public void SaveData(GameData data)
        {
            data.ItemIDs = new List<string>();

            foreach (ItemSO item in _items)
            {
                if (data.ItemIDs.Contains(item.ID))
                {
                    Debug.LogError("Duplicate item ID detected during save.");
                    continue;
                }
                data.ItemIDs.Add(item.ID);
            }
        }
        #endregion
    }
}