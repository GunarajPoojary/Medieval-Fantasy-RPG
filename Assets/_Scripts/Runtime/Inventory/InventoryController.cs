using RPG.Utilities.Inputs.ScriptableObjects;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Inventory
{
    public class InventoryController
    {
        private readonly Inventory _model;
        private readonly UIInventory _view;
        private readonly InputReader _input;
        private bool _isInventoryOpen = false;

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
            _model.OnInventoryItemAdded += OnInventoryItemAdded;
        }

        public void RemoveListeners()
        {
            _input.InventoryAction -= ToggleInventory;
            _model.OnInventoryFull -= OnInventoryFull;
            _model.OnItemNull -= OnItemNull;
            _model.OnInvalidQuantity -= OnInvalidQuantity;
            _model.OnItemStackLimitReached -= OnItemStackLimitReached;
            _model.OnInventoryItemAdded -= OnInventoryItemAdded;
        }

        private void ToggleInventory()
        {
            _view.ToggleInventory();

            _isInventoryOpen = !_isInventoryOpen;

            if (_isInventoryOpen)
                _input.DisablePlayerMovementActions();
            else
                _input.EnablePlayerMovementActions();
        }


        private void OnInventoryItemAdded(InventoryItem item)
        {
            _view.AddSlotUI(item);
        }

        private void OnItemStackLimitReached(string name) => _view.OnItemStackLimitReached(name);

        private void OnInvalidQuantity(int quantity) => _view.OnInvalidQuantity(quantity);

        private void OnItemNull() => _view.OnItemNull();

        private void OnInventoryFull() => _view.OnInventoryFull();

        public void AddItem(ItemSO item) => _model.AddItem(item);

        public void RemoveItem(string itemName)
        {
            _model.RemoveItem(itemName);
        }
    }
}