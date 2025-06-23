using UnityEngine;

namespace ProjectEmbersteel.StateMachine
{
    /// <summary>
    /// Defines the contract for a state in a state machine, typically used for player.
    /// Each state handles its own lifecycle, including entering, exiting, and responding to input,
    /// updates, physics, collisions, and animation events.
    /// </summary>
    public interface IState
    {
        public void Enter();
        public void Exit();
        public void HandleInput();
        public void UpdateState();
        public void PhysicsUpdate();
        public void OnTriggerEnter(Collider collider);
        public void OnTriggerExit(Collider collider);

        /// <summary>
        /// Called at the beginning of an animation event.
        /// </summary>
        public void OnAnimationEnterEvent();

        /// <summary>
        /// Called at the end of an animation event.
        /// </summary>
        public void OnAnimationExitEvent();

        /// <summary>
        /// Called during a transition between two animations.
        /// </summary>
        public void OnAnimationTransitionEvent();
    }
}