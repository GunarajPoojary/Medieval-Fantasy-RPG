using RPG.ScriptableObjects.EventChannels;
using RPG.ScriptableObjects.Items;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventories
{
    /// <summary>
    /// Class that serves as a database for storing and managing item data.
    /// </summary>
    public class ItemDataBase : MonoBehaviour
    {
        [SerializeField] private List<WearableSO> _wearables = new List<WearableSO>();
        [Space]
        [SerializeField] private List<WeaponSO> _weapons = new List<WeaponSO>();
        [Space]
        [SerializeField] private ItemSOReturnStringParameterEventChannelSO _itemLookupEvent;  // Event channel for item lookup

        private Dictionary<string, ItemSO> _itemLookup = new Dictionary<string, ItemSO>();

        private bool _isInitialized = false;

        private void OnValidate()
        {
            if (Application.isPlaying)
            {
                return;
            }

            InitializeItemLookup();
        }


        private void OnEnable() => _itemLookupEvent.OnEventRaised += GetItemByID;

        private void OnDisable() => _itemLookupEvent.OnEventRaised -= GetItemByID;

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                InitializeItemLookup();
                _isInitialized = true;
            }
        }

        private void InitializeItemLookup()
        {
            _itemLookup.Clear();

            AddItemsToLookup(_weapons);
            AddItemsToLookup(_wearables);

            _isInitialized = true;
        }

        private ItemSO GetItemByID(string id)
        {
            EnsureInitialized();

            return _itemLookup.TryGetValue(id, out var item) ? item : null;
        }

        private void AddItemsToLookup<T>(List<T> items) where T : ItemSO
        {
            foreach (var item in items)
            {
                if (item == null) continue;

                if (_itemLookup.ContainsKey(item.ID))
                {
                    Debug.LogWarning($"Duplicate item ID detected: {item.ID}. Item '{item.name}' was not added.");
                    continue;
                }

                _itemLookup[item.ID] = item;
            }
        }
    }
}