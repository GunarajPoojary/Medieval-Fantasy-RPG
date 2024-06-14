using GunarajCode.Inventories;
using UnityEngine;

namespace GunarajCode.ScriptableObjects
{
    public interface IGuidGenerator
    {
        void GenerateGuid();
    }

    public enum ItemType { Weapon, Armor, Consumable, QuestItem }

    [CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Items/Item")]
    public class ItemObject : ScriptableObject, IGuidGenerator
    {
        public string DisplayName;
        public Sprite Icon;
        [TextArea(15, 20)]
        public string Description;
        public ItemType Type;
        public string ID;

        public void GenerateGuid()
        {
            ID = System.Guid.NewGuid().ToString();
        }

        public virtual void Use()
        {
            Debug.Log("Using " + DisplayName);
        }

        public void RemoveFromInventory() => Inventory.Instance.Remove(this);
    }
}
