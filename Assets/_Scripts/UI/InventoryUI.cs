using GunarajCode.Inventories;
using GunarajCode.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GunarajCode
{
    /// <summary>
    /// This class manages the Inventory UI, allowing items to be displayed and interacted with.
    /// It handles adding items to the UI, toggling the visibility of the inventory, and responding to user input.
    /// </summary>
    public class InventoryUI : MonoBehaviour
    {
        private PlayerInputsAction _inputActions;

        private Inventory _inventory;

        [SerializeField] private InventoryItemInfo _inventoryItemInfo;
        [SerializeField] private Transform _weaponsContainer;
        [SerializeField] private Transform _armorsContainer;
        [SerializeField] private Transform _questItemsContainer;
        [SerializeField] private Transform _questItemContainer;

        [SerializeField] private GameObject _inventorySlotPrefab;

        [SerializeField] private GameObject _inventoryContainer;

        private Dictionary<ItemType, Transform> _itemTypeToContainerMap;

        private void Awake()
        {
            _inventory = Inventory.Instance;

            _inputActions = new PlayerInputsAction();

            _itemTypeToContainerMap = new Dictionary<ItemType, Transform>
            {
                { ItemType.Weapon, _weaponsContainer },
                { ItemType.Armor, _armorsContainer },
                { ItemType.Consumable, _questItemsContainer },
                { ItemType.QuestItem, _questItemContainer }
            };
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();
            _inputActions.Player.Inventory.performed += ToggleInventory;

            if (_inventory != null)
                _inventory.OnItemAdded += OnItemAdded;
        }

        private void OnDisable()
        {
            _inputActions.Player.Inventory.performed -= ToggleInventory;
            _inputActions.Player.Disable();

            if (_inventory != null)
                _inventory.OnItemAdded -= OnItemAdded;
        }

        private void Start() => _inventoryContainer.SetActive(false);

        // Toggle the visibility of the inventory UI when the corresponding input action is performed.
        private void ToggleInventory(InputAction.CallbackContext context)
        {
            bool isActive = _inventoryContainer.activeSelf;
            _inventoryContainer.SetActive(!isActive);
        }

        // Add a new item to the appropriate UI container when it is added to the inventory.
        private void OnItemAdded(ItemObject item)
        {
            if (_itemTypeToContainerMap.TryGetValue(item.Type, out Transform container))
                AddItemToContainer(container, item);
            else
                AddItemToContainer(_questItemContainer, item);
        }

        // Instantiate a new inventory slot, set its item, and subscribe to the button press event.
        private void AddItemToContainer(Transform container, ItemObject item)
        {
            ItemSlot slot = Instantiate(_inventorySlotPrefab, container).GetComponent<ItemSlot>();
            slot.SetItem(item);
            slot.OnButtonPressed += _inventoryItemInfo.OnButtonPressed;
        }
    }
}
