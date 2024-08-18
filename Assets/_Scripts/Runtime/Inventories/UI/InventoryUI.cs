using RPG.ScriptableObjects.EventChannels;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.Inventories.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GameObject _inventoryPanel;
        [Space]
        [SerializeField] private VoidReturnItemSOParameterEventChannelSO _itemAdded;  // Event channel for item addition
        [Space]
        [SerializeField] private ItemSOListReturnNonParameterEventChannelSO _getItems;  // Event channel to get items

        private IInventorySlotInstantiator _inventorySlotManager;
        private IInventoryInputHandler _inputHandler;

        private void Awake()
        {
            _inputHandler = new InventoryInputHandler(_inventoryPanel);
            _inventorySlotManager = GetComponent<InventorySlotsInstantiator>();
        }

        private void OnEnable()
        {
            _inventoryPanel.SetActive(false);
            _inputHandler.Enable();
            _itemAdded.OnEventRaised += HandleItemAdd;
        }

        private void Start()
        {
            foreach (var item in _getItems.RaiseEvent())
            {
                HandleItemAdd(item);
            }
        }

        private void OnDisable()
        {
            _inputHandler.Disable();
            _itemAdded.OnEventRaised -= HandleItemAdd;
        }

        private void HandleItemAdd(ItemSO item) => _inventorySlotManager.AddItemToUI(item);
    }
}