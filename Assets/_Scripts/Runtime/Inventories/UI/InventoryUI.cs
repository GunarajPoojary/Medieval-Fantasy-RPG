using RPG.ScriptableObjects.EventChannels;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.Inventories.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [SerializeField] private GameObject _inventoryPanel;

        [SerializeField] private VoidReturnItemSOParameterEventChannelSO _itemAdded;  // Event channel for item addition

        [SerializeField] private ItemSOListReturnNonParameterEventChannelSO _getItems;  // Event channel to get items

        private IInventorySlotManager _inventorySlotManager;
        private IInputHandler _inputHandler;

        private void Awake()
        {
            _inputHandler = new InventoryInputHandler(_inventoryPanel);
            _inventorySlotManager = GetComponent<InventorySlotManager>();
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

        private void HandleItemAdd(ItemSO item) => _inventorySlotManager.AddItem(item);
    }
}