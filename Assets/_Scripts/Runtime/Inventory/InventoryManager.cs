using RPG.Events.EventChannel;
using RPG.Loot;
using RPG.UI.Inventory;
using UnityEngine;

namespace RPG.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        [Header("Inventory Components")]
        [Tooltip("The view component that displays the inventory UI")]
        [SerializeField] private UIInventory _view;
        [SerializeField] private IPickableEventChannelSO _onItemPickedEventChannel = default;
        [field: SerializeField] public Inventory Model { get; private set; }
        private IPickable _pickable = null;
        private InventoryController _controller;

        private void Awake()
        {
            _controller = new InventoryController(Model, _view);
        }

        private void OnEnable()
        {
            _controller.AddListeners();
            _controller.OnItemAdded += OnItemPickupSuccess;
            _controller.OnItemAddFail += ResetState;
            _onItemPickedEventChannel.OnEventRaised += TryAddItem;
        }

        private void OnDisable()
        {
            _controller.RemoveListeners();
            _controller.OnItemAdded -= OnItemPickupSuccess;
            _controller.OnItemAddFail -= ResetState;
            _onItemPickedEventChannel.OnEventRaised -= TryAddItem;
        }

        public void OnItemPickupSuccess()
        {
            _pickable?.SetGameObject(false);

            ResetState();
        }

        public void ToggleInventory(bool setActive) => _controller.ToggleInventory(setActive);

        private void TryAddItem(IPickable pickable)
        {
            _pickable = pickable;

            _controller.AddItem(pickable.PickUpItem());
        }

        private void ResetState() => _pickable = null;

        private void RemoveItem(string itemName) => _controller.RemoveItem(itemName);
    }
}