using System;

namespace ProjectEmbersteel.StateMachine
{
    public class GameStateMachine
    {
        private GameState _currentState;

        public Action InventoryEnterAction;
        public Action DialogueEnterAction;
        public Action CombatEnterAction;

        public Action InventoryExitAction;
        public Action DialogueExitAction;
        public Action CombatExitAction;

        public void AddEnterActionCallbacks(Action inventoryEnterAction, Action dialogueEnterAction, Action combatEnterAction)
        {
            InventoryEnterAction += inventoryEnterAction;
            DialogueEnterAction += dialogueEnterAction;
            CombatEnterAction += combatEnterAction;
        }

        public void AddExitActionCallbacks(Action inventoryExitAction, Action dialogueExitAction, Action combatExitAction)
        {
            InventoryExitAction += inventoryExitAction;
            DialogueExitAction += dialogueExitAction;
            CombatExitAction += combatExitAction;
        }

        public void RemoveEnterActionCallbacks(Action inventoryEnterAction, Action dialogueEnterAction, Action combatEnterAction)
        {
            InventoryEnterAction -= inventoryEnterAction;
            DialogueEnterAction -= dialogueEnterAction;
            CombatEnterAction -= combatEnterAction;
        }

        public void RemoveExitActionCallbacks(Action inventoryExitAction, Action dialogueExitAction, Action combatExitAction)
        {
            InventoryExitAction -= inventoryExitAction;
            DialogueExitAction -= dialogueExitAction;
            CombatExitAction -= combatExitAction;
        }

        public void SwitchState(GameState newState)
        {
            if (_currentState == newState)
            {
                if (newState != GameState.Gameplay)
                    SwitchState(GameState.Gameplay);
                return;
            }

            ExitState(_currentState);
            EnterState(newState);
        }

        public void EnterState(GameState newState)
        {
            switch (newState)
            {
                case GameState.Inventory:
                    InventoryEnterAction?.Invoke();
                    break;
                case GameState.Dialogue:
                    DialogueEnterAction?.Invoke();
                    break;
                case GameState.Combat:
                    CombatEnterAction?.Invoke();
                    break;
            }

            _currentState = newState;
        }

        public void ExitState(GameState state)
        {
            switch (state)
            {
                case GameState.Inventory:
                    InventoryExitAction?.Invoke();
                    break;
                case GameState.Dialogue:
                    DialogueExitAction?.Invoke();
                    break;
                case GameState.Combat:
                    CombatExitAction?.Invoke();
                    break;
            }
        }
    }
}