using RPG.Core;
using RPG.SaveLoad;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Gameplay.Inventories
{
    /// <summary>
    /// Manages the player's inventory, allowing items to be added and removed.
    /// Inherits from a Singleton base class to ensure only one instance exists.
    /// </summary>
    public class Inventory : GenericSingleton<Inventory>, ISaveable
    {
        /// <summary>
        /// List of items currently in the inventory.
        /// </summary>
        public List<ItemSO> Items { get; set; } = new List<ItemSO>();

        /// <summary>
        /// Maximum capacity of the inventory.
        /// </summary>
        [SerializeField]
        private int _maxCapacity = 100;

        /// <summary>
        /// Event triggered when an item is added to the inventory.
        /// </summary>
        public event Action<ItemSO> OnItemAdded;

        /// <summary>
        /// Event triggered when an item is removed from the inventory.
        /// </summary>
        public event Action<ItemSO> OnItemRemoved;

        /// <summary>
        /// Adds an item to the inventory.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddItem(ItemSO item)
        {
            if (Items.Count >= _maxCapacity)
            {
                Debug.LogWarning("Inventory is full. Cannot add more items.");
                return;
            }

            if (item == null)
            {
                Debug.LogWarning("Attempted to add a null item to the inventory.");
                return;
            }

            Items.Add(item);
            OnItemAdded?.Invoke(item);
        }

        /// <summary>
        /// Removes an item from the inventory.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        public void RemoveItem(ItemSO item)
        {
            if (item == null)
            {
                Debug.LogWarning("Attempted to remove a null item from the inventory.");
                return;
            }

            if (Items.Remove(item))
            {
                OnItemRemoved?.Invoke(item);
            }
            else
            {
                Debug.LogWarning("Item not found in the inventory.");
            }
        }

        /// <summary>
        /// Loads inventory data from the specified game data.
        /// </summary>
        /// <param name="data">The game data to load from.</param>
        public void LoadData(GameData data)
        {
            Items = new List<ItemSO>();
            foreach (string itemID in data.ItemIDs)
            {
                ItemSO item = ItemDataBase.Instance.GetItemByID(itemID);
                if (item != null)
                {
                    Items.Add(item);
                }
                else
                {
                    Debug.LogWarning($"Item with ID {itemID} not found in the database.");
                }
            }
        }

        /// <summary>
        /// Saves inventory data to the specified game data.
        /// </summary>
        /// <param name="data">The game data to save to.</param>
        public void SaveData(GameData data)
        {
            data.ItemIDs = new List<string>();
            foreach (ItemSO item in Items)
            {
                data.ItemIDs.Add(item.ID);
            }
        }
    }
}
