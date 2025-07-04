using System;
using ProjectEmbersteel.Item;
using ProjectEmbersteel.UI.Inventory;
using ProjectEmbersteel.Utilities.Inputs.ScriptableObjects;

namespace ProjectEmbersteel.Inventory
{
    /// <summary>
    /// Class that manages communication between inventory model and view
    /// </summary>
    public class InventoryController
    {
        private readonly Inventory _model;
        private readonly UIInventory _view;
        private readonly InputReader _input;

        public event Action OnItemAdded;
        public event Action OnItemAddFail;

        public InventoryController(InputReader input, Inventory model, UIInventory view)
        {
            _input = input;
            _model = model;
            _view = view;
        }

        public void AddListeners()
        {
            _input.InventoryAction += ToggleInventory;

            _model.OnInventoryFull += OnInventoryFull;
            _model.OnItemNull += OnItemNull;
            _model.OnInvalidQuantity += OnInvalidQuantity;
            _model.OnItemStackLimitReached += OnItemStackLimitReached;
            _model.OnItemAdded += OnInventoryItemAdded;
        }
        
        public void RemoveListeners()
        {
            _input.InventoryAction -= ToggleInventory;

            _model.OnInventoryFull -= OnInventoryFull;
            _model.OnItemNull -= OnItemNull;
            _model.OnInvalidQuantity -= OnInvalidQuantity;
            _model.OnItemStackLimitReached -= OnItemStackLimitReached;
            _model.OnItemAdded -= OnInventoryItemAdded;
        }

        public void ToggleInventory() => _view.ToggleInventory();
        public void ToggleInventory(bool toggle) => _view.ToggleInventory(toggle);

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