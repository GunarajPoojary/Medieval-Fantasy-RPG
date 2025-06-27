using System;
using System.Collections.Generic;
using ProjectEmbersteel.Item;
using UnityEngine;

namespace ProjectEmbersteel.Inventory
{
    /// <summary>
    /// Inventory model class that manages collections of items
    /// </summary>
    [Serializable]
    public class Inventory
    {
        [Range(1, 2000)] public int MaxCapacity = 100;

        private readonly Dictionary<string, InventoryItem> _itemsById = new();
        private int _currentCapacity = 0;

        #region Events
        public event Action OnItemNull;
        public event Action<int> OnInvalidQuantity;
        public event Action OnInventoryFull;
        public event Action<string> OnItemStackLimitReached;
        public event Action<InventoryItem> OnItemAdded;
        #endregion

        /// <summary>
        /// Adds an item to the inventory with specified quantity
        /// Handles capacity checking, stacking logic, and appropriate event notifications
        /// </summary>
        /// <param name="worldItem">The item scriptable object to add to the inventory</param>
        /// <param name="quantity">Number of items to add (default: 1)</param>
        public void AddItem(ItemSO worldItem, int quantity = 1)
        {
            if (worldItem == null)
            {
                OnItemNull?.Invoke();
                return;
            }

            if (quantity < 1)
            {
                OnInvalidQuantity?.Invoke(quantity);
                return;
            }

            if (_currentCapacity >= MaxCapacity)
            {
                OnInventoryFull?.Invoke();
                return;
            }

            // Calculate how many items can actually be added based on available capacity
            int remaining = quantity;
            int availableCapacity = MaxCapacity - _currentCapacity;

            // Limit quantity to available capacity to prevent overflow
            remaining = Mathf.Min(remaining, availableCapacity);

            // Handle stackable items by attempting to add to existing stacks
            if (worldItem.IsStackable && _itemsById.TryGetValue(worldItem.ID, out InventoryItem existingItem))
            {
                // Attempt to add items to existing stack
                int leftover = existingItem.PushStack(remaining);
                int actuallyAdded = remaining - leftover;
                _currentCapacity += actuallyAdded;

                // Handle case where not all items could be stacked due to stack limits
                if (leftover > 0)
                    OnItemStackLimitReached?.Invoke(worldItem.DisplayName);

                OnItemAdded?.Invoke(existingItem);
            }
            else
            {
                // Create new inventory item entry for non-stackable items or new item types
                InventoryItem newItem = new InventoryItem(worldItem, remaining);
                _itemsById[worldItem.ID] = newItem;
                _currentCapacity += remaining;

                OnItemAdded?.Invoke(newItem);
            }
        }

        /// <summary>
        /// Method to remove items from inventory by name and quantity
        /// Currently returns false as implementation is pending
        /// </summary>
        /// <param name="itemName">Name of the item to remove from inventory</param>
        /// <param name="quantity">Number of items to remove (default: 1)</param>
        /// <returns>True if items were successfully removed, false otherwise</returns>
        public bool RemoveItem(string itemName, int quantity = 1)
        {
            // TODO: Implement item removal logic
            // Should handle quantity validation, item existence checking, and capacity updates
            return false;
        }
    }
}