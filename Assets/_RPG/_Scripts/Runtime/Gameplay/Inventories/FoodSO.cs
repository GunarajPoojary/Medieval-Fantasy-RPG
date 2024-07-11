using UnityEngine;

namespace RPG.Gameplay.Inventories
{
    /// <summary>
    /// Represents a food item in the inventory system.
    /// Inherits from ItemSO and adds additional properties specific to food items.
    /// </summary>
    [CreateAssetMenu(fileName = "New Food Item", menuName = "Inventory/Items/Food", order = 3)]
    public class FoodSO : ItemSO
    {
        public int Health;

        /// <summary>
        /// Uses the food item. Override this method to provide specific functionality for consuming food.
        /// </summary>
        public override void Use()
        {
            // Implement food consumption logic here
        }

        /// <summary>
        /// Validates the food item settings in the Unity editor.
        /// Sets the item type in Unity editor.
        /// </summary>

#if UNITY_EDITOR
        private void OnValidate() => Type = ItemType.Consumable;
#endif
    }
}
