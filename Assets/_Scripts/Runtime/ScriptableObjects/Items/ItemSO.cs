using UnityEngine;

namespace RPG
{
    public enum ItemType
    {
        Default,
        Weapon,
        Wearable,
        Consumable,
        QuestItem
    }

    public abstract class ItemSO : ScriptableObject
    {
        public string Name;
        public Sprite Icon;

        [Multiline(5)]
        public string Description;
        public ItemType Type;

        [Tooltip("Use context menu to generate GUID")]
        public string ID;

        [ContextMenu("Generate GUID")]
        public void GenerateGuid() => ID = System.Guid.NewGuid().ToString();
    }
}