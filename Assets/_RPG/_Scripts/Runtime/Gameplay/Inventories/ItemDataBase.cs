using RPG.Core;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Gameplay.Inventories
{
    public class ItemDataBase : GenericSingleton<ItemDataBase>
    {
        [SerializeField] private ArmorSO[] _defaultSkins = new ArmorSO[4];
        [SerializeField] private List<ArmorSO> _armors;
        [SerializeField] private List<WeaponSO> _weapons;

        private Dictionary<string, ItemSO> _itemLookup;

        protected override void Awake()
        {
            base.Awake();
            InitializeItemLookup();
        }

        private void InitializeItemLookup()
        {
            _itemLookup = new Dictionary<string, ItemSO>();

            AddItemsToLookup(_weapons);
            AddItemsToLookup(_defaultSkins);
            AddItemsToLookup(_armors);
        }

        private void AddItemsToLookup<T>(IEnumerable<T> items) where T : ItemSO
        {
            foreach (var item in items)
            {
                if (item == null) continue;

                if (_itemLookup.ContainsKey(item.ID))
                {
                    Debug.LogWarning($"Duplicate ID found: {item.ID}. Item {item.name} not added to the database.");
                    continue;
                }
                _itemLookup[item.ID] = item;
            }
        }

        public ItemSO GetItemByID(string id)
        {
            if (_itemLookup.TryGetValue(id, out var item))
            {
                return item;
            }
            else
            {
                Debug.LogWarning($"Item with ID {id} not found.");
                return null;
            }
        }
    }
}