using RPG.Core.SaveLoad;
using RPG.ScriptableObjects.EventChannels;
using RPG.ScriptableObjects.Items;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    public class Inventory : MonoBehaviour, ISaveable
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

        private void Awake()
        {
            Debug.Assert(_itemLookupEvent != null, "_itemLookupEvent is not assigned.");
            Debug.Assert(_addItem != null, "_addItem is not assigned.");
            Debug.Assert(_removeItem != null, "_removeItem is not assigned.");
            Debug.Assert(_itemAdded != null, "_itemAdded is not assigned.");
            Debug.Assert(_itemRemoved != null, "_itemRemoved is not assigned.");
            Debug.Assert(_getItems != null, "_getItems is not assigned.");
        }

        private void OnEnable()
        {
            _addItem.OnEventRaised += AddItem;
            _removeItem.OnEventRaised += RemoveItem;
            _getItems.OnEventRaised += GetItems;
        }

        private void OnDisable()
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
                Debug.LogWarning("Inventory is full.");
                return;
            }

            if (item == null)
            {
                Debug.LogWarning("Cannot add a null item.");
                return;
            }

            if (_items.Contains(item))
            {
                Debug.LogWarning("Item is already in the inventory.");
                return;
            }

            _items.Add(item);
            _itemAdded.RaiseEvent(item);
        }

        public void RemoveItem(ItemSO item)
        {
            if (item == null)
            {
                Debug.LogWarning("Item is null, cannot remove.");
                return;
            }

            if (_items.Remove(item))
            {
                _itemRemoved.RaiseEvent(item);
            }
        }

        #region ISaveable Methods
        public void LoadData(GameData data)
        {
            _items.Clear();

            foreach (string itemID in data.ItemIDs)
            {
                ItemSO item = _itemLookupEvent.RaiseEvent(itemID);

                if (item != null)
                {
                    _items.Add(item);
                }
            }
        }

        public void SaveData(GameData data)
        {
            data.ItemIDs.Clear();

            foreach (ItemSO item in _items)
            {
                data.ItemIDs.Add(item.ID);
            }
        }
        #endregion
    }
}