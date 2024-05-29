using GunarajCode.Gameplay;
using UnityEngine;

namespace GunarajCode
{
    public class NPC : MonoBehaviour, IInteractable
    {
        public Transform InteractableTransform => throw new System.NotImplementedException();

        public void Interact()
        {
            Debug.Log("Hello");
        }
    }
}
