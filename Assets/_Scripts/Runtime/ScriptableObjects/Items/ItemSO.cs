using System;
using UnityEngine;

namespace ProjectEmbersteel.Item
{
    public enum ItemType
    {
        OneHandedSword,
        GreatSword,
        BowAndArrow,
        HeadArmor,
        ArmArmor,
        ChestArmor,
        BeltArmor,
        LegArmor,
        FeetArmor,
        Edible,
        Potion,
        Ore,
        SpecialItem
    }

    public abstract class ItemSO : DescriptionBaseSO
    {
        [Header("Basic Info")]
        [SerializeField] private string _displayName;
        [SerializeField] private Sprite _icon;
        [SerializeField, TextArea] private string _itemDescription;

        [Header("Classification")]
        [SerializeField] protected ItemType _type;

        [Header("Stack Settings")]
        [SerializeField] private bool _isStackable = false;
        [SerializeField] private bool _isSellable = true;
        [SerializeField, Range(1, 100)] private int _maxStack = 1;

        [HideInInspector]
        [SerializeField] private string _id;

        public string DisplayName => _displayName;
        public Sprite Icon => _icon;
        public string ItemDescription => _itemDescription;
        public bool IsStackable => _isStackable;
        public bool IsSellable => _isSellable;
        public int MaxStack => _isStackable ? _maxStack : 1;
        public ItemType Type => _type;
        public string ID => _id;

        protected virtual void OnValidate()
        {
#if UNITY_EDITOR
            if (string.IsNullOrEmpty(_id))
            {
                _id = Guid.NewGuid().ToString("N");
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
        }
    }
}