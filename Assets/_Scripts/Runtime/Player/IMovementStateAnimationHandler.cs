using UnityEngine;

namespace RPG
{
    /// <summary>
    /// Interface for handling animation events related to player movement states.
    /// </summary>
    public interface IMovementStateAnimationEventsHandler
    {
        Animator Animator { get; }

        void OnMovementStateAnimationEnterEvent();
        void OnMovementStateAnimationExitEvent();
        void OnMovementStateAnimationTransitionEvent();
    }
}