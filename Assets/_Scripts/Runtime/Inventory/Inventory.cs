using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventory
{
    [Serializable]
    public class Inventory
    {
        [Range(1, 2000)] public int MaxCapacity = 100;

        private readonly List<InventoryItem> _items = new();

        public event Action OnItemNull;
        public event Action<int> OnInvalidQuantity;
        public event Action OnInventoryFull;
        public event Action<string> OnItemStackLimitReached;
        public event Action<InventoryItem> OnInventoryItemAdded;

        public void AddItem(ItemSO item, int quantity = 1)
        {
            if (item == null)
            {
                OnItemNull?.Invoke();
                return;
            }

            if (quantity <= 0)
            {
                OnInvalidQuantity?.Invoke(quantity);
                return;
            }

            int currentCapacity = GetCurrentCapacity();
            if (currentCapacity >= MaxCapacity)
            {
                OnInventoryFull?.Invoke();
                return;
            }

            int remaining = quantity;

            // Try stacking into existing items
            if (item.IsStackable && _items != null)
            {
                foreach (var existing in _items)
                {
                    if (existing.CanStack(item))
                    {
                        remaining = existing.PushStack(remaining);
                        OnInventoryItemAdded?.Invoke(existing);
                        PickableItemManager.Instance.OnSuccessfulItemPickup(item);

                        if (remaining <= 0)
                            return;
                    }
                }
            }

            // Add new stacks with remaining quantity
            while (remaining > 0 && GetCurrentCapacity() < MaxCapacity)
            {
                int stackToAdd = Mathf.Min(remaining, item.MaxStack);
                InventoryItem newItem = new(item, stackToAdd);
                _items.Add(newItem);
                OnInventoryItemAdded?.Invoke(newItem);
                PickableItemManager.Instance.OnSuccessfulItemPickup(item);
                remaining -= stackToAdd;
            }

            if (remaining > 0)
            {
                OnItemStackLimitReached?.Invoke(item.DisplayName);
            }
        }

        public bool RemoveItem(string itemName, int quantity = 1)
        {
            // Not implemented
            return false;
        }

        private int GetCurrentCapacity()
        {
            int total = 0;
            foreach (var item in _items)
                total += item.StackCount;
            return total;
        }
    }
}