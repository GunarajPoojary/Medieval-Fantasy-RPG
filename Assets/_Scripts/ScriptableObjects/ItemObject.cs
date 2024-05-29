using GunarajCode.Inventories;
using UnityEngine;

namespace GunarajCode.ScriptableObjects
{
    public enum ItemType { Weapon, Armor, Consumable, QuestItem }

    public abstract class ItemObject : ScriptableObject
    {
        public string DisplayName;
        public Sprite Icon;
        [TextArea(15, 20)]
        public string Description;
        public ItemType Type;

        /// <summary>
        /// Item's use behavior.
        /// </summary>
        public abstract void Use();

        /// <summary>
        /// Removes the item from the inventory.
        /// </summary>
        public void RemoveFromInventory() => Inventory.Instance.Remove(this);
    }
}