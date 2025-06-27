using System;
using System.Collections.Generic;
using ProjectEmbersteel.Events.EventChannel;
using ProjectEmbersteel.Inventory;
using ProjectEmbersteel.Item;
using ProjectEmbersteel.StatSystem;
using ProjectEmbersteel.Utilities;
using UnityEngine;

namespace ProjectEmbersteel.UI.Inventory
{
    [Serializable]
    public struct ItemContentPanel
    {
        public ItemType Type;
        public Transform ContentPanel;
    }
    public class UIInventory : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _inventoryPanel;
        [SerializeField] private ItemContentPanel[] _itemContentPanels;
        [SerializeField] private UIInventorySlot _slotPrefab;

        [Header("Item Details Panel")]
        [SerializeField] private UIInventoryItemOverview _itemOverviewPanel;
        [SerializeField] private UIInventoryResponsePopup _uiInventoryResponsePopup;
        [Header("Listening to")]
        [SerializeField] private StatUpdateEventChannel _statUpdateEventChannel;
        [SerializeField] private PlayerAttributesUI _attributesUI;
        [SerializeField] private AudioSource _uiAudioSource;

        [Header("Configuration")]
        [SerializeField] private int _poolSize = 100;

        // Object pooling
        private ObjectPool<UIInventorySlot> _slotsPool;
        private RectTransform _itemPoolContainer;

        // Data structures for efficient inventory management
        private readonly Dictionary<ItemType, List<UIInventorySlot>> _slotUIsByType = new();
        private readonly Dictionary<ItemSO, UIInventorySlot> _stackableItemSlots = new();
        private Dictionary<ItemType, Transform> _itemContentPanelsMap;

        #region Unity Lifecycle
        private void Awake()
        {
            ValidateSerializedFields();
            InitializeInventory();
            SetContentPanelsByItemType();
        }

        private void OnEnable() => SubscribeToEvents(true);

        private void OnDisable() => SubscribeToEvents(false);

        private void OnDestroy() => CleanupSlotEvents();
        #endregion

        #region Initialization
        private void ValidateSerializedFields()
        {
#if UNITY_EDITOR
            if (_inventoryPanel == null)
                Debug.LogError("Inventory Panel is not assigned", this);

            if (_slotPrefab == null)
                Debug.LogError("Slot Prefab is not assigned", this);

            if (_itemOverviewPanel == null)
                Debug.LogError("Item Overview Panel is not assigned", this);

            if (_uiInventoryResponsePopup == null)
                Debug.LogError("Inventory Response Popup is not assigned", this);
#endif
        }

        private void InitializeInventory()
        {
            _inventoryPanel.SetActive(false);

            CreateSlotPoolContainer();
            InitializeObjectPool();
        }

        private void SubscribeToEvents(bool subscribe)
        {
            if (subscribe)
                _statUpdateEventChannel.OnEventRaised += OnUpdateBaseStats;
            else
                _statUpdateEventChannel.OnEventRaised -= OnUpdateBaseStats;
        }

        private void OnUpdateBaseStats(StatType statType, Stat stat) => _attributesUI.OnUpdateBaseStats(statType, stat);

        private void CreateSlotPoolContainer()
        {
            GameObject containerGO = new GameObject("InventorySlotPool", typeof(RectTransform));
            _itemPoolContainer = containerGO.GetComponent<RectTransform>();
            _itemPoolContainer.SetParent((RectTransform)transform);
        }

        private void InitializeObjectPool()
        {
            _slotsPool = new ObjectPool<UIInventorySlot>(
                createFunc: CreateSlotObject,
                onGet: OnSlotRetrieved,
                onRelease: OnSlotReleased,
                poolSize: _poolSize
            );
        }

        private void SetContentPanelsByItemType()
        {
            _itemContentPanelsMap = new Dictionary<ItemType, Transform>(_itemContentPanels.Length);

            foreach (ItemContentPanel itemContentEntry in _itemContentPanels)
                _itemContentPanelsMap[itemContentEntry.Type] = itemContentEntry.ContentPanel;
        }
        #endregion

        #region Public Interface
        public void AddSlotUI(InventoryItem item)
        {
            if (!ValidateInventoryItem(item)) return;

            ItemType itemType = item.Item.Type;

            if (!_itemContentPanelsMap.TryGetValue(itemType, out Transform contentPanel))
            {
                Debug.LogError($"No container found for item type: {itemType}");
                return;
            }

            EnsureSlotListExists(itemType);

            if (item.Item.IsStackable)
                HandleStackableItem(item, itemType, contentPanel);
            else
                HandleNonStackableItem(item, itemType, contentPanel);
        }

        public void ToggleInventory(bool setActive)
        {
            _inventoryPanel.SetActive(setActive);
        }
        #endregion

        #region Item Management
        private bool ValidateInventoryItem(InventoryItem item)
        {
            if (item?.Item == null)
            {
                OnItemNull();
                return false;
            }
            return true;
        }

        private void EnsureSlotListExists(ItemType itemType)
        {
            if (!_slotUIsByType.ContainsKey(itemType))
                _slotUIsByType[itemType] = new List<UIInventorySlot>();
        }

        private void HandleStackableItem(InventoryItem item, ItemType itemType, Transform container)
        {
            if (_stackableItemSlots.TryGetValue(item.Item, out UIInventorySlot existingSlot))
            {
                existingSlot.UpdateStackCount();
                return;
            }

            UIInventorySlot newSlot = CreateAndSetupSlot(item, itemType, container);
            _stackableItemSlots[item.Item] = newSlot;
        }

        private void HandleNonStackableItem(InventoryItem item, ItemType itemType, Transform container)
        {
            CreateAndSetupSlot(item, itemType, container);
        }

        private UIInventorySlot CreateAndSetupSlot(InventoryItem item, ItemType itemType, Transform container)
        {
            UIInventorySlot slot = _slotsPool.Get();
            slot.Initialize(item, container, _uiAudioSource);
            slot.OnClick += OnItemClicked;

            _slotUIsByType[itemType].Add(slot);
            return slot;
        }
        #endregion

        #region Event Handlers
        private void OnItemClicked(ItemSO item) => _itemOverviewPanel.DisplayItemOverview(item);
        #endregion

        #region Popup Notifications
        public void OnItemStackLimitReached(string itemName) => DisplayInventoryResponsePopup($"{itemName} stack limit has been reached.");

        public void OnInvalidQuantity(int quantity) => DisplayInventoryResponsePopup($"Provided quantity ({quantity}) is not valid.");

        public void OnInventoryFull() => DisplayInventoryResponsePopup("Inventory is full.");

        public void OnItemNull() => DisplayInventoryResponsePopup("Provided item is null.");

        private void DisplayInventoryResponsePopup(string message) => _uiInventoryResponsePopup?.ShowPopup(message);
        #endregion

        #region Object Pool Management
        private UIInventorySlot CreateSlotObject()
        {
            UIInventorySlot slot = Instantiate(_slotPrefab, _itemPoolContainer);
            slot.gameObject.SetActive(false);
            return slot;
        }

        private void OnSlotRetrieved(UIInventorySlot slot) => slot.gameObject.SetActive(true);

        private void OnSlotReleased(UIInventorySlot slot)
        {
            slot.gameObject.SetActive(false);
            slot.OnClick -= OnItemClicked; // Prevent memory leaks
        }
        #endregion

        #region Cleanup
        private void CleanupSlotEvents()
        {
            foreach (List<UIInventorySlot> slotList in _slotUIsByType.Values)
            {
                foreach (UIInventorySlot slot in slotList)
                {
                    if (slot != null)
                        slot.OnClick -= OnItemClicked;
                }
            }
        }
        #endregion
    }
}