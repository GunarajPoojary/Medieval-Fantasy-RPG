using System;
using RPG.EquipmentSystem;
using RPG.Inventory;
using RPG.Utilities.Inputs.ScriptableObjects;

namespace RPG.StateMachine
{
    public class GameStateMachine
    {
        private readonly InventoryManager _inventoryManager;
        private readonly InputReader _input;
        public GameState CurrentState { get; private set; }
        public event Action<GameState> OnStateChanged;
        private bool _isInventoryOpen = false;

        public GameStateMachine(InputReader input, InventoryManager inventoryManager)
        {
            _inventoryManager = inventoryManager;
            _input = input;
        }

        public void AddInputActionCallbacks()
        {
            _input.InventoryAction += ToggleInventoryState;
            _input.EquipmentMenuAction += ToggleEquipmentState;
        }

        public void RemoveInputActionCallbacks()
        {
            _input.InventoryAction -= ToggleInventoryState;
            _input.EquipmentMenuAction -= ToggleEquipmentState;
        }

        private void ToggleEquipmentState() => SwitchState(GameState.EquipmentMenu);

        private void ToggleInventoryState() => SwitchState(GameState.Inventory);

        public void SwitchState(GameState newState)
        {
            if (CurrentState == newState)
            {
                // Toggle behavior (e.g., close menu and return to gameplay)
                if (newState != GameState.Gameplay)
                    SwitchState(GameState.Gameplay);
                return;
            }

            ExitState(CurrentState);
            EnterState(newState);
        }

        public void EnterState(GameState newState)
        {
            switch (newState)
            {
                case GameState.Inventory:
                    _inventoryManager.ToggleInventory(true);
                    _input.DisablePlayerMovementActions();
                    break;
                case GameState.ShopMenu:
                    //ShopMenu.Instance.Open();
                    break;
                case GameState.Settings:
                    //SettingsMenu.Instance.Open();
                    break;
                case GameState.Combat:
                    // Enable combat UI
                    break;
            }

            CurrentState = newState;
            OnStateChanged?.Invoke(CurrentState);
        }

        public void ExitState(GameState state)
        {
            switch (state)
            {
                case GameState.Inventory:
                    _inventoryManager.ToggleInventory(false);
                    _input.EnablePlayerMovementActions();
                    break;
                case GameState.ShopMenu:
                    //ShopMenu.Instance.Close();
                    break;
                case GameState.Settings:
                    //SettingsMenu.Instance.Close();
                    break;
                case GameState.Combat:
                    // Disable combat UI
                    break;
            }
        }
    }
}
