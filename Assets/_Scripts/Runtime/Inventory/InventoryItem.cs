using UnityEngine;

namespace RPG.Inventory
{
    [System.Serializable]
    public class InventoryItem
    {
        public ItemSO Item { get; private set; }
        public int StackCount { get; private set; }
        public bool IsEmpty => Item == null || StackCount <= 0;
        public bool IsStackable => Item != null && Item.IsStackable;

        public InventoryItem(ItemSO item, int stackCount = 1)
        {
            Item = item;

            if (item == null)
            {
                StackCount = 0;
                return;
            }

            StackCount = Mathf.Clamp(stackCount, 1, item.MaxStack);
        }

        public bool CanStack(ItemSO item)
        {
            if (item == null || Item == null) return false;

            return Item.ID == item.ID &&
                   Item.IsStackable &&
                   StackCount < Item.MaxStack;
        }

        /// <summary>
        /// Adds to the stack. Returns the leftover amount that couldn't be added.
        /// </summary>
        public int PushStack(int amount)
        {
            if (amount <= 0 || Item == null || StackCount >= Item.MaxStack)
                return amount;

            int maxAddable = Item.MaxStack - StackCount;
            int toAdd = Mathf.Min(amount, maxAddable);
            StackCount += toAdd;
            return amount - toAdd;
        }

        public void PopStack(int amount)
        {
            if (amount <= 0) return;

            StackCount = Mathf.Max(0, StackCount - amount);

            if (StackCount <= 0) ClearStack();
        }

        public void ClearStack()
        {
            Item = null;
            StackCount = 0;
        }

        public void SetStackCount(int newStackCount)
        {
            StackCount = Mathf.Clamp(newStackCount, 0, Item?.MaxStack ?? 0);

            if (StackCount <= 0) ClearStack();
        }
    }
}