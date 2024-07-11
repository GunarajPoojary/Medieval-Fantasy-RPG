using UnityEngine;

namespace RPG.Gameplay.Inventories
{
    /// <summary>
    /// Interface for generating GUIDs.
    /// </summary>
    public interface IGUIDGenerator
    {
        /// <summary>
        /// Generates a new GUID.
        /// </summary>
        void GenerateGuid();
    }

    /// <summary>
    /// Enumeration of different item types.
    /// </summary>
    public enum ItemType
    {
        Default,
        Weapon,
        Armor,
        Consumable,
        QuestItem
    }

    /// <summary>
    /// Abstract base class for items in the inventory system.
    /// Inherits from ScriptableObject and implements IGUIDGenerator.
    /// </summary>
    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Items/Item")]
    public abstract class ItemSO : ScriptableObject, IGUIDGenerator
    {
        public string DisplayName;
        public Sprite Icon;

        [Multiline(5)]
        public string Description;
        public ItemType Type;
        public string ID;

        public abstract void Use();

        public void GenerateGuid() => ID = System.Guid.NewGuid().ToString();

        public void RemoveFromInventory() => Inventory.Instance.RemoveItem(this);
    }
}
