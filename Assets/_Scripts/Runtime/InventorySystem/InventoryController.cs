using System;
using ProjectEmbersteel.Item;
using ProjectEmbersteel.UI.Inventory;

namespace ProjectEmbersteel.Inventory
{
    /// <summary>
    /// Class that manages communication between inventory model and view
    /// </summary>
    public class InventoryController
    {
        private readonly Inventory _model;
        private readonly UIInventory _view;

        public event Action OnItemAdded;
        public event Action OnItemAddFail;

        public InventoryController(Inventory model, UIInventory view)
        {
            _model = model;
            _view = view;
        }

        /// <summary>
        /// Method to subscribe to all necessary events and input actions
        /// </summary>
        public void AddListeners()
        {
            _model.OnInventoryFull += OnInventoryFull;
            _model.OnItemNull += OnItemNull;
            _model.OnInvalidQuantity += OnInvalidQuantity;
            _model.OnItemStackLimitReached += OnItemStackLimitReached;
            _model.OnItemAdded += OnInventoryItemAdded;
        }

        /// <summary>
        /// Method to unsubscribe from all events and input actions to prevent memory leaks
        /// </summary>
        public void RemoveListeners()
        {
            _model.OnInventoryFull -= OnInventoryFull;
            _model.OnItemNull -= OnItemNull;
            _model.OnInvalidQuantity -= OnInvalidQuantity;
            _model.OnItemStackLimitReached -= OnItemStackLimitReached;
            _model.OnItemAdded -= OnInventoryItemAdded;
        }

        public void ToggleInventory(bool setActive) => _view.ToggleInventory(setActive);

        // Event handler for when items are added to the inventory
        private void OnInventoryItemAdded(InventoryItem item)
        {
            _view.AddSlotUI(item);
            OnItemAdded?.Invoke();
        }

        // Forward stack limit reached event to the view with item name
        private void OnItemStackLimitReached(string name)
        {
            _view.OnItemStackLimitReached(name);
            OnItemAddFail?.Invoke();
        }

        // Forward invalid quantity event to the view with the problematic quantity value
        private void OnInvalidQuantity(int quantity)
        {
            _view.OnInvalidQuantity(quantity);
            OnItemAddFail?.Invoke();
        }

        // Forward null item event to the view for user notification
        private void OnItemNull()
        {
            _view.OnItemNull();
            OnItemAddFail?.Invoke();
        }

        // Forward inventory full event to the view for user notification
        private void OnInventoryFull()
        {
            _view.OnInventoryFull();
            OnItemAddFail?.Invoke();
        }

        // Public method to add items to the inventory by delegating to the model
        public void AddItem(ItemSO item) => _model.AddItem(item);

        // Public method to remove items from the inventory by name
        public void RemoveItem(string itemName)
        {
            _model.RemoveItem(itemName);
        }
    }
}