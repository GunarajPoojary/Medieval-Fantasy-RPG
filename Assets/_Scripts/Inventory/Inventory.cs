using GunarajCode.ScriptableObjects;
using System;
using System.Collections.Generic;

namespace GunarajCode.Inventories
{
    /// <summary>
    /// Manages the player's inventory, allowing items to be added and removed.
    /// Inherits from a Singleton base class to ensure only one instance exists.
    /// </summary>
    public class Inventory : Singleton<Inventory>, IDataPersistence
    {
        private List<ItemObject> _items = new List<ItemObject>();
        public List<ItemObject> Items { get { return _items; } set { _items = value; } }

        public event Action<ItemObject> OnItemAdded;

        public event Action<ItemObject> OnItemRemoved;

        public void Add(ItemObject item)
        {
            _items.Add(item);
            OnItemAdded?.Invoke(item);
        }
        public void Remove(ItemObject item)
        {
            _items.Remove(item);
            OnItemRemoved?.Invoke(item);
        }

        public void LoadData(GameData data)
        {
            _items.Clear();
            foreach (var id in data.ItemIDs)
            {
                var item = ItemDatabase.Instance.GetItemByID(id);
                if (item != null)
                {
                    _items.Add(item);
                }
            }
        }

        public void SaveData(GameData data)
        {
            data.ItemIDs.Clear();
            foreach (var item in _items)
            {
                data.ItemIDs.Add(item.ID);  // Use DisplayName or another unique identifier
            }
        }
    }
}
