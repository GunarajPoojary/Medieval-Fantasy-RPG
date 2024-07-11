using UnityEngine;

namespace RPG.Gameplay.Inventories
{
    /// <summary>
    /// Represents a default item in the inventory system.
    /// Inherits from ItemSO and implements specific functionality for default items.
    /// </summary>
    [CreateAssetMenu(fileName = "New Default Item", menuName = "Inventory/Items/Default")]
    public class DefaultSO : ItemSO
    {
        /// <summary>
        /// Uses the default item.
        /// Override this method to provide specific functionality for default items.
        /// </summary>
        public override void Use()
        {
            // Implement default item usage logic here
        }

        /// <summary>
        /// Validates the default item settings in the Unity editor.
        /// Sets the item type in Unity editor.
        /// </summary>

#if UNITY_EDITOR
        private void OnValidate() => Type = ItemType.Default;
#endif
    }
}
