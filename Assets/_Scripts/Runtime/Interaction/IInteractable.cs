using UnityEngine;

namespace RPG.Gameplay.World
{
    public interface IInteractable
    {
        void Interact();

        Transform GetTransform();
    }
}