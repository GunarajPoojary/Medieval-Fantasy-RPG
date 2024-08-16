using RPG.Core.Utils;
using RPG.ScriptableObjects.EventChannels;
using RPG.ScriptableObjects.Items;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// Class that serves as a database for storing and managing item data.
    /// </summary>
    public class ItemDataBase : SimpleSingleton<ItemDataBase>
    {
        [SerializeField] private List<WearableSO> _wearables;
        [Space]
        [SerializeField] private List<WeaponSO> _weapons;
        [Space]
        [SerializeField] private ItemSOReturnStringParameterEventChannelSO _itemLookupEvent;  // Event channel for item lookup

        private Dictionary<string, ItemSO> _itemLookup;

        private void OnValidate() => InitializeItemLookup();

        private void OnEnable() => _itemLookupEvent.OnEventRaised += GetItemByID;

        private void OnDestroy() => _itemLookupEvent.OnEventRaised -= GetItemByID;

        public ItemSO GetItemByID(string id)
        {
            if (_itemLookup.TryGetValue(id, out var item))
            {
                return item;
            }
            else
            {
                return null;
            }
        }

        private void InitializeItemLookup()
        {
            _itemLookup = new Dictionary<string, ItemSO>();

            AddItemsToLookup(_weapons);
            AddItemsToLookup(_wearables);
        }

        private void AddItemsToLookup<T>(IEnumerable<T> items) where T : ItemSO
        {
            HashSet<string> duplicateChecker = new HashSet<string>();

            foreach (var item in items)
            {
                if (item == null || !duplicateChecker.Add(item.ID))
                {
                    continue;
                }

                _itemLookup[item.ID] = item;
            }
        }
    }
}