using RPG.EquipmentSystem;
using RPG.Inventory;
using RPG.StateMachine;
using RPG.UI;
using RPG.Utilities.Inputs.ScriptableObjects;
using UnityEngine;

namespace RPG
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private UIManager _uIManager;
        [SerializeField] private InventoryManager _inventoryManager;
        [SerializeField] private InputReader _input;
        private GameStateMachine _gameStateMachine;

        private void Awake() => InitializeGameStateMachine();

        private void OnEnable() => AddStateMachineInputActionCallbacks();

        private void OnDisable() => RemoveStateMachineInputActionCallbacks();

        private void InitializeGameStateMachine()
        {
            _gameStateMachine = new GameStateMachine(_input, _inventoryManager);
            _gameStateMachine.EnterState(GameState.Gameplay);
        }

        private void AddStateMachineInputActionCallbacks() => _gameStateMachine.AddInputActionCallbacks();
        private void RemoveStateMachineInputActionCallbacks() => _gameStateMachine.RemoveInputActionCallbacks();
    }
}