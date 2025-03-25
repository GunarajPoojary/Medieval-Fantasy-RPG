using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    public class Inventory : MonoBehaviour//, ISaveable
    {
        [SerializeField] private int _maxCapacity = 200;

        private List<ItemSO> _items = new();

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
        }

        public void RemoveItem(ItemSO item)
        {
            if (item == null)
            {
                Debug.LogWarning("Item is null, cannot remove.");
                return;
            }

            _items.Remove(item);
        }

        // #region ISaveable Methods
        // public void LoadData(GameData data)
        // {
        //     _items.Clear();
        //
        //     foreach (string itemID in data.ItemIDs)
        //     {
        //         ItemSO item = null; //_itemLookupEvent.RaiseEvent(itemID);
        //
        //         if (item != null)
        //         {
        //             _items.Add(item);
        //         }
        //     }
        // }
        //
        // public void SaveData(GameData data)
        // {
        //     data.ItemIDs.Clear();
        //
        //     foreach (ItemSO item in _items)
        //     {
        //         data.ItemIDs.Add(item.ID);
        //     }
        // }
        // #endregion
    }
}