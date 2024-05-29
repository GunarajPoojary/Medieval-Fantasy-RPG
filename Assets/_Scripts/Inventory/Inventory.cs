using GunarajCode.ScriptableObjects;
using System;
using System.Collections.Generic;

namespace GunarajCode.Inventories
{
    /// <summary>
    /// Manages the player's inventory, allowing items to be added and removed.
    /// Inherits from a Singleton base class to ensure only one instance exists.
    /// </summary>
    public class Inventory : Singleton<Inventory>
    {
        private List<ItemObject> _items = new List<ItemObject>();

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
    }
}
