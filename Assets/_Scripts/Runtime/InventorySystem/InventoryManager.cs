using System;
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

        private void Awake() => CreateInventoryController();

        private void OnEnable()
        {
            AddControllerListeners();
            SubscribeToControllerEvents();
        }

        private void OnDisable()
        {
            RemoveControllerListeners();
            UnsubscribeToControllerEvents();
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

        private void CreateInventoryController() => _controller = new InventoryController(Model, _view);
        
        private void AddControllerListeners() => _controller.AddListeners();
        private void RemoveControllerListeners() => _controller.RemoveListeners();

        private void SubscribeToControllerEvents()
        {
            _controller.OnItemAdded += OnItemPickupSuccess;
            _controller.OnItemAddFail += ResetState;
            _onItemPickedEventChannel.OnEventRaised += TryAddItem;
        }

        private void UnsubscribeToControllerEvents()
        {
            _controller.OnItemAdded -= OnItemPickupSuccess;
            _controller.OnItemAddFail -= ResetState;
            _onItemPickedEventChannel.OnEventRaised -= TryAddItem;
        }

        private void ResetState() => _pickable = null;

        private void RemoveItem(string itemName) => _controller.RemoveItem(itemName);
    }
}