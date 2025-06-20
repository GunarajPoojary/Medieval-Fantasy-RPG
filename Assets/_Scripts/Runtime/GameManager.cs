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
        [SerializeField] private PlayerEquipmentManager _playerEquipmentManager;
        [SerializeField] private InputReader _input;
        private GameStateMachine _gameStateMachine;

        private void Awake()
        {
            _gameStateMachine = new GameStateMachine(_input, _inventoryManager, _playerEquipmentManager);
            _gameStateMachine.EnterState(GameState.Gameplay);
        }

        private void OnEnable()
        {
            _gameStateMachine.AddInputActionCallbacks();
        }

        private void OnDisable()
        {
            _gameStateMachine.RemoveInputActionCallbacks();
        }
    }
}