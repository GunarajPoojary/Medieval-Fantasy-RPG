using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GameObject _inventoryPanel;
        [SerializeField] private PlayerInputHandler _input;

        [SerializedDictionary("Item Type", "Item Panel")]
        [SerializeField] private SerializedDictionary<ItemType, Transform> _itemTypeToPanelsMap;

        [SerializeField] private InventorySlotUI _inventorySlotPrefab;
        private readonly List<InventorySlotUI> _inventorySlots = new();

        private void Awake()
        {
            if (_itemTypeToPanelsMap.ContainsKey(ItemType.Default))
            {
                _itemTypeToPanelsMap.Remove(ItemType.Default);
            }
        }

        private void OnEnable() => _input.InventoryAction.performed += ToggleInventory;

        private void OnDisable() => _input.InventoryAction.performed -= ToggleInventory;

        private void ToggleInventory(InputAction.CallbackContext ctx) => _inventoryPanel.SetActive(!_inventoryPanel.activeSelf);

        public void HandleItemAdd(ItemSO item)
        {
            if (!_itemTypeToPanelsMap.TryGetValue(item.Type, out var container)) return;
            
            var inventorySlot = Instantiate(_inventorySlotPrefab, container);
            inventorySlot.Initialize(item);
            _inventorySlots.Add(inventorySlot);
        }
    }
}