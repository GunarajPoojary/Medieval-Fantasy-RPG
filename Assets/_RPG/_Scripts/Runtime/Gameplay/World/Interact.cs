using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Gameplay.World
{
    /// <summary>
    /// Player's interaction with interactable objects.
    /// </summary>
    public class Interact : MonoBehaviour
    {
        private GameInputAction _inputActions;
        [SerializeField] private float _sphereRadius;
        [SerializeField] private LayerMask _interactable;
        public event Action<IInteractable> OnGetInteractable;

        private void Awake()
        {
            _inputActions = new GameInputAction();
            _inputActions.Player.Enable();

            _inputActions.Player.Interact.performed += ItemInteract_performed;
        }

        private void OnDestroy() => _inputActions.Player.Interact.performed -= ItemInteract_performed;

        private void ItemInteract_performed(InputAction.CallbackContext context)
        {
            IInteractable interactable = GetInteractableObject();
            interactable?.Interact();
        }

        private void Update()
        {
            // Get the closest interactable object and invoke the OnGetInteractable event
            IInteractable interactable = GetInteractableObject();
            OnGetInteractable?.Invoke(interactable);
        }

        public IInteractable GetInteractableObject()
        {
            List<IInteractable> interactableList = GetInteractableList();

            return FindClosestInteractable(interactableList);
        }

        private List<IInteractable> GetInteractableList()
        {
            var interactableList = new List<IInteractable>();
            var hitColliders = Physics.OverlapSphere(transform.position, _sphereRadius, _interactable);

            foreach (var collider in hitColliders)
            {
                if (collider.TryGetComponent(out IInteractable interactable))
                    interactableList.Add(interactable);
            }

            return interactableList;
        }

        private IInteractable FindClosestInteractable(List<IInteractable> interactableList)
        {
            IInteractable closestInteractable = null;

            foreach (IInteractable interactable in interactableList)
            {
                if (closestInteractable == null || IsInteractableCloser(interactable, closestInteractable))
                    closestInteractable = interactable;
            }

            return closestInteractable;
        }

        // Method to determine if a given interactable is closer to the player than another interactable
        private bool IsInteractableCloser(IInteractable interactable, IInteractable closestInteractable) => Vector3.Distance(transform.position, interactable.InteractableTransform.position) < Vector3.Distance(transform.position, closestInteractable.InteractableTransform.position);
    }
}