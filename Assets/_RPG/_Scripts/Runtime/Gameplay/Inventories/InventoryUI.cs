using AYellowpaper.SerializedCollections;
using RPG.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Gameplay.Inventories
{
    /// <summary>
    /// This class manages the Inventory UI, allowing items to be displayed and interacted with.
    /// It handles adding items to the UI, toggling the visibility of the inventory, and responding to user input.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private InventoryItemInfo _inventoryItemInfo;

        [SerializeField] private GameObject _inventorySlotPrefab;

        [SerializeField] private GameObject _inventoryContainer;

        [SerializedDictionary("Item Type", "Item Container")]
        [SerializeField] private SerializedDictionary<ItemType, Transform> _itemTypeToContainerMap = new SerializedDictionary<ItemType, Transform>();

        private void Awake()
        {
            _inventoryContainer.SetActive(false);

            InputManager.Instance.UIActions.Inventory.started += ToggleInventory;
        }

        private void Start()
        {
            if (Inventory.Instance != null)
            {
                Inventory.Instance.OnItemAdded += InstantiateSlot;
                foreach (var item in Inventory.Instance.Items)
                    InstantiateSlot(item);
            }
        }

        private void OnDisable()
        {
            Inventory.Instance.OnItemAdded -= InstantiateSlot;
            InputManager.Instance.UIActions.Inventory.started -= ToggleInventory;
        }

        // Toggle the visibility of the inventory UI when the corresponding input action is performed.
        private void ToggleInventory(InputAction.CallbackContext context)
        {
            bool isActive = _inventoryContainer.activeSelf;

            Time.timeScale = isActive ? 1.0f : 0.0f;

            _inventoryContainer.SetActive(!isActive);
        }

        // Add a new item to the appropriate UI container when it is added to the inventory.
        private void InstantiateSlot(ItemSO item)
        {
            if (_itemTypeToContainerMap.TryGetValue(item.Type, out Transform container))
                AddItemToContainer(container, item);
        }

        // Instantiate a new inventory slot, set its item, and subscribe to the button press event.
        private void AddItemToContainer(Transform container, ItemSO item)
        {
            InventorySlotUI slot = Instantiate(_inventorySlotPrefab, container).GetComponent<InventorySlotUI>();
            slot.SetItem(item);
            slot.OnSlotSelected += _inventoryItemInfo.OnSlotSelected;
        }
    }
}
