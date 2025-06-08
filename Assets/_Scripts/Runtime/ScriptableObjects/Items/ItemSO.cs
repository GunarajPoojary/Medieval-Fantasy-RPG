using UnityEngine;

namespace RPG
{
    public enum ItemType
    {
        Weapon,
        Wearable,
        Consumable,
        QuestItem
    }

    [CreateAssetMenu(fileName = "NewItem", menuName = "Game/Inventory/Item", order = 1)]
    public class ItemSO : DescriptionBaseSO
    {
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;

        [SerializeField, TextArea] private string _itemDescription;
        [SerializeField] protected ItemType _type;
        [SerializeField] private bool _isStackable = false;
        [SerializeField, Range(1, 100)] private int _maxStack = 1;

        [SerializeField, HideInInspector] private string _id;

        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
        public string ItemDescription => _itemDescription;
        public bool IsStackable => _isStackable;
        public int MaxStack => _isStackable ? _maxStack : 1;
        public ItemType Type => _type;
        public string ID => _id;

        protected virtual void OnValidate()
        {
            if (string.IsNullOrEmpty(_id))
            {
                _id = System.Guid.NewGuid().ToString("N");
                UnityEditor.EditorUtility.SetDirty(this);
            }
        }
    }
}