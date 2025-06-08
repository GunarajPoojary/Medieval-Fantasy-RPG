using RPG.Utilities.Inputs.ScriptableObjects;
using UnityEngine;

namespace RPG.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        [Header("Inventory Components")]
        [Tooltip("The view component that displays the inventory UI")]
        [SerializeField] private UIInventory _view;
        [SerializeField] private Inventory _model;
        [SerializeField] private InputReader _input;
        [SerializeField] private ItemSOEventChannelSO _onItemPickedEventChannel = default;

        private InventoryController _controller;

        private void Awake()
        {
            _controller = new InventoryController(_input, _model, _view);
        }

        private void OnEnable()
        {
            _controller.AddListeners();
            _onItemPickedEventChannel.OnEventRaised += AddItem;
        }

        private void OnDisable()
        {
            _controller.RemoveListeners();
            _onItemPickedEventChannel.OnEventRaised -= AddItem;
        }

        public void CloseInventory() => _input.EnablePlayerMovementActions();

        private void AddItem(ItemSO item) => _controller.AddItem(item);

        private void RemoveItem(string itemName) => _controller.RemoveItem(itemName);
    }
}