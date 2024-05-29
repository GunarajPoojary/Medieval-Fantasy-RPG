using UnityEngine;

namespace GunarajCode.Gameplay
{
    /// <summary>
    /// Interface for interactable objects in the game.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// Performs an interaction with the object.
        /// </summary>
        void Interact();

        /// <summary>
        /// Gets the transform of the interactable object.
        /// </summary>
        Transform InteractableTransform { get; }
    }
}
