using RPG.Gameplay.World;
using UnityEngine;

namespace RPG.Gameplay.NPC
{
    public class NPCInteract : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            Debug.Log("NPC interaction isn't implemented yet");
        }

        public Transform GetTransform() => transform;
    }
}