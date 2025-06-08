using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Inventory
{
    [System.Serializable]
    public class ItemContentPanel
    {
        public ItemType ItemType;
        public Transform Transform;
    }

    public class UIInventory : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _inventoryPanel;
        [SerializeField] private List<ItemContentPanel> _itemContentPanels;
        [SerializeField] private UIInventorySlot _slotPrefab;
        [SerializeField] private TabGroup _tabs;

        [Header("Item Details Panel")]
        [SerializeField] private UIInventoryItemOverview _itemOverviewPanel;
        [SerializeField] private UIInventoryResponsePopup _uiInventoryResponsePopup;

        private ObjectPool<UIInventorySlot> _slotsPool;
        private Transform _itemPoolContainer;

        private readonly Dictionary<ItemType, List<UIInventorySlot>> _slotUIsByType = new();
        private readonly Dictionary<ItemSO, int> _stackableItems = new();
        private Dictionary<ItemType, Transform> _contentPanelsByType;

        private void Awake()
        {
            _inventoryPanel.SetActive(false);

            _itemPoolContainer = new GameObject("ItemPoolContainer").transform;
            _slotsPool = new ObjectPool<UIInventorySlot>(CreateSlotObj, OnGetSlot, OnReleaseSlot, 100);

            // Build dictionary from serialized panel list
            _contentPanelsByType = new Dictionary<ItemType, Transform>();
            foreach (var panel in _itemContentPanels)
            {
                if (!_contentPanelsByType.ContainsKey(panel.ItemType))
                    _contentPanelsByType.Add(panel.ItemType, panel.Transform);
            }

#if UNITY_EDITOR
            if (_uiInventoryResponsePopup == null)
                Debug.LogError("You forgot to assign Inventory Response Popup to Inventory UI");
#endif
        }

        private void OnEnable() => _tabs.OnTabChanged += OnTabChanged;

        private void OnDisable() => _tabs.OnTabChanged -= OnTabChanged;

        public void AddSlotUI(InventoryItem item)
        {
            if (item == null || item.Item == null)
                return;

            ItemType type = item.Item.Type;

            if (!_slotUIsByType.ContainsKey(type))
                _slotUIsByType[type] = new List<UIInventorySlot>();

            if (!_contentPanelsByType.TryGetValue(type, out Transform container))
            {
                Debug.LogWarning($"No UI container defined for item type: {type}");
                return;
            }

            List<UIInventorySlot> slotList = _slotUIsByType[type];

            if (item.Item.IsStackable && type == ItemType.Consumable)
            {
                if (_stackableItems.TryGetValue(item.Item, out int index))
                {
                    slotList[index].UpdateStackCount();
                    return;
                }

                AddSlot(slotList, container, item);
                _stackableItems.Add(item.Item, slotList.Count - 1);
            }
            else
            {
                AddSlot(slotList, container, item);
            }
        }

        public void OnItemStackLimitReached(string name) => DisplayInventoryResponsePopup($"{name} stack limit has been reached.");

        public void OnInvalidQuantity(int quantity) => DisplayInventoryResponsePopup($"Provided quantity ({quantity}) is not valid.");

        public void OnInventoryFull() => DisplayInventoryResponsePopup("Inventory is full.");

        public void OnItemNull() => DisplayInventoryResponsePopup("Provided item is null.");

        // Optional cleanup method
        public void ClearAllSlots()
        {
            foreach (var slotList in _slotUIsByType.Values)
            {
                foreach (var slot in slotList)
                    _slotsPool.Release(slot);
                    
                slotList.Clear();
            }

            _stackableItems.Clear();
        }

        private void OnTabChanged(int index) => ShowFirstItemForTab(index);

        private void ShowFirstItemForTab(int index)
        {
            ItemType targetType = (ItemType)index;

            if (_slotUIsByType.TryGetValue(targetType, out List<UIInventorySlot> slots) &&
                slots != null && slots.Count > 0)
                OnItemClicked(slots[0].ItemSO); // Tab has items - show the first item
            else
                _itemOverviewPanel.ResetSelection(); // Tab has no items - show default overview
        }

        private void AddSlot(List<UIInventorySlot> slotUIs, Transform container, InventoryItem item)
        {
            UIInventorySlot slot = _slotsPool.Get();
            slot.Initialize(item, container);
            slot.ItemClicked += OnItemClicked;
            slotUIs.Add(slot);
        }

        private void OnItemClicked(ItemSO item) => _itemOverviewPanel.SetSelectedItem(item);

        public void ToggleInventory()
        {
            if (_inventoryPanel != null)
                _inventoryPanel.SetActive(!_inventoryPanel.activeSelf);

            if (_inventoryPanel.activeSelf)
                InitializeInventoryDisplay();
        }

        private void InitializeInventoryDisplay()
        {
            // Find the first tab that has items
            ItemType[] itemTypes = (ItemType[])Enum.GetValues(typeof(ItemType));
            int firstTabWithItems = -1;

            for (int i = 0; i < itemTypes.Length; i++)
            {
                if (_slotUIsByType.TryGetValue(itemTypes[i], out List<UIInventorySlot> slots) &&
                    slots != null && slots.Count > 0)
                {
                    firstTabWithItems = i;
                    break;
                }
            }

            if (firstTabWithItems >= 0)
            {
                // Set tab and show first item (without triggering events that cause recursion)
                _tabs.SetDefaultTab(firstTabWithItems);
                ShowFirstItemForTab(firstTabWithItems);
            }
            else
            {
                // No items in any tab - default to first tab with default overview
                _tabs.SetDefaultTab(0);
                _itemOverviewPanel.ResetSelection();
            }
        }

        private void DisplayInventoryResponsePopup(string message) => _uiInventoryResponsePopup.ShowPopup(message);

        private UIInventorySlot CreateSlotObj()
        {
            UIInventorySlot slot = Instantiate(_slotPrefab, _itemPoolContainer);
            slot.gameObject.SetActive(false);
            return slot;
        }

        private void OnGetSlot(UIInventorySlot slot) => slot.gameObject.SetActive(true);
        private void OnReleaseSlot(UIInventorySlot slot) => slot.gameObject.SetActive(false);

        private void UseSelectedItem()
        {
            // TODO: implement interaction with controller
        }

        private void DestroySelectedItem()
        {
            // TODO: implement item destruction logic
        }
    }
}