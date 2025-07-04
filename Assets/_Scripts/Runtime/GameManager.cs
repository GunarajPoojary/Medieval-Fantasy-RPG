using System;
using ProjectEmbersteel.Events.EventChannel;
using ProjectEmbersteel.Inventory;
using ProjectEmbersteel.StateMachine;
using ProjectEmbersteel.UI;
using ProjectEmbersteel.Utilities.Inputs.ScriptableObjects;
using UnityEngine;

namespace ProjectEmbersteel
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private UIManager _uIManager;
        [SerializeField] private InventoryManager _inventoryManager;
        [SerializeField] private InputReader _input;

        [Header("Listening on")]
        [SerializeField] private DialogueEventChannelSO _startDialogueEventChannel;
        [SerializeField] private VoidEventChannelSO _endDialogueEventChannel;
        [SerializeField] private VoidEventChannelSO _openInventoryEventChannel;
        [SerializeField] private VoidEventChannelSO _closeInventoryEventChannel;

        private GameStateMachine _gameStateMachine;

        private void Awake() => InitializeGameStateMachine();

        private void OnEnable()
        {
            _openInventoryEventChannel.OnEventRaised += SwitchToInventoryState;
            _closeInventoryEventChannel.OnEventRaised += SwitchToGameplayState;

            _gameStateMachine.AddEnterActionCallbacks(
                _input.DisablePlayerMovementActions,
                _input.DisablePlayerMovementActions,
                _input.DisablePlayerMovementActions);
                
            _gameStateMachine.AddExitActionCallbacks(
                InventoryExitAction,
                _input.EnablePlayerMovementActions,
                _input.EnablePlayerMovementActions);
        }

        private void OnDisable()
        {
            _openInventoryEventChannel.OnEventRaised -= SwitchToInventoryState;
            _closeInventoryEventChannel.OnEventRaised -= SwitchToGameplayState;

            _gameStateMachine.RemoveEnterActionCallbacks(
                _input.DisablePlayerMovementActions,
                _input.DisablePlayerMovementActions,
                _input.DisablePlayerMovementActions);

            _gameStateMachine.RemoveExitActionCallbacks(
                InventoryExitAction,
                _input.EnablePlayerMovementActions,
                _input.EnablePlayerMovementActions);
        }

        private void InitializeGameStateMachine() => _gameStateMachine = new GameStateMachine();

        private void SwitchToInventoryState() => _gameStateMachine.SwitchState(GameState.Inventory);

        private void SwitchToGameplayState() => _gameStateMachine.SwitchState(GameState.Gameplay);

        private void InventoryExitAction()
        {
            _inventoryManager.ToggleInventory(false);
            _input.EnablePlayerMovementActions();
        }
    }
}