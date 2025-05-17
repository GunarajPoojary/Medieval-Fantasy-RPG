using UnityEngine;

namespace RPG
{
    public interface IState
    {
        public void Enter();
        public void Exit();
        public void HandleInput();
        public void Update();
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