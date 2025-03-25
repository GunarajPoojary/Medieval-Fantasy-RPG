using UnityEngine;

namespace RPG
{
    public abstract class StateMachine
    {
        protected IState _currentState;

        public void ChangeState(IState newState)
        {
            _currentState?.Exit();

            _currentState = newState;

            _currentState.Enter();
        }

        public void HandleInput() => _currentState?.HandleInput();

        public void Update()
        {
            //Debug.Log(_currentState.GetType().Name);
            _currentState?.Update();
        }

        public void PhysicsUpdate() => _currentState?.PhysicsUpdate();

        public void OnTriggerEnter(Collider collider) => _currentState?.OnTriggerEnter(collider);

        public void OnTriggerExit(Collider collider) => _currentState?.OnTriggerExit(collider);

        public void OnAnimationEnterEvent() => _currentState?.OnAnimationEnterEvent();

        public void OnAnimationExitEvent() => _currentState?.OnAnimationExitEvent();

        public void OnAnimationTransitionEvent() => _currentState?.OnAnimationTransitionEvent();
    }
}