using ProjectEmbersteel.Item;
using UnityEngine;

namespace ProjectEmbersteel.Inventory
{
    /// <summary>
    /// Class that represents Inventory item(Both stackable and Non-Stackable)
    /// </summary>
    [System.Serializable]
    public class InventoryItem
    {
        private ItemSO _item;
        private int _stackCount;

        public ItemSO Item => _item;
        public int StackCount => _stackCount;
        public int RemainingStackSize => _item.MaxStack - _stackCount;

        public InventoryItem(ItemSO item, int stackCount = 1)
        {
            if (item == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Item is null");
#endif
                return;
            }

            _item = item;

            _stackCount = Mathf.Clamp(stackCount, 1, item.MaxStack);
        }

        /// <summary>
        /// Method to Reset the Inventory Item to orignal state(Ex: Used for pool)
        /// </summary>
        /// <param name="item"></param>
        /// <param name="stackCount"></param>
        public void ResetItem(ItemSO item, int stackCount = 1)
        {
            if (item == null)
            {
#if UNITY_EDITOR
                Debug.LogError("Item is null");
#endif
                return;
            }

            _item = item;

            _stackCount = Mathf.Clamp(stackCount, 1, item.MaxStack);
        }

        /// <summary>
        /// Adds to the stack
        /// </summary>
        /// <returns> Leftover amount that couldn't be added </returns>
        /// <param name="amount"></param>
        public int PushStack(int amount)
        {
            if (amount <= 0) return 0;
            int toAdd = Mathf.Min(amount, RemainingStackSize);
            _stackCount += toAdd;
            return amount - toAdd;
        }

        /// <summary>
        /// // Method to remove items from the stack
        /// </summary>
        /// <param name="amount"></param>
        public void PopStack(int amount)
        {
            if (amount <= 0) return;

            _stackCount = Mathf.Max(0, _stackCount - amount); // Subtract the amount while preventing negative stack counts
        }
    }
}