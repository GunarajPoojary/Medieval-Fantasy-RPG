using RPG.InputActions;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Gameplay.World
{
    /// <summary>
    /// Handles player interaction with objects in the world, detecting and interacting with nearby interactable objects.
    /// </summary>
    public class Interact : MonoBehaviour
    {
        [SerializeField] private float _detectionRadius;
        [SerializeField] private LayerMask _interactable;

        private PlayerInputActions _inputActions;

        public event Action<IInteractable> OnGetInteractable;

        private void Awake()
        {
            _inputActions = new PlayerInputActions();
            _inputActions.Player.Enable();

            _inputActions.Player.Interact.performed += OnItemInteractPerformed;
        }

        private void OnDestroy() => _inputActions.Player.Interact.performed -= OnItemInteractPerformed;

        private void Update()
        {
            IInteractable interactable = GetInteractableObject();
            OnGetInteractable?.Invoke(interactable);
        }

        // Finds the closest interactable object within the detection radius.
        public IInteractable GetInteractableObject()
        {
            List<IInteractable> interactableList = GetInteractableList();

            return FindClosestInteractable(interactableList);
        }

        private void OnItemInteractPerformed(InputAction.CallbackContext context)
        {
            // Get the closest interactable object and trigger its interaction
            IInteractable interactable = GetInteractableObject();
            interactable?.Interact();
        }

        // Gets a list of all interactable objects within the detection radius.
        private List<IInteractable> GetInteractableList()
        {
            var interactableList = new List<IInteractable>();

            // Detect all colliders within the specified radius and layer mask
            var hitColliders = Physics.OverlapSphere(transform.position, _detectionRadius, _interactable);

            foreach (var collider in hitColliders)
            {
                // If the collider has an IInteractable component, add it to the list
                if (collider.TryGetComponent(out IInteractable interactable))
                {
                    interactableList.Add(interactable);
                }
            }

            return interactableList;
        }

        // Finds the closest interactable object from a list.
        private IInteractable FindClosestInteractable(List<IInteractable> interactableList)
        {
            IInteractable closestInteractable = null;

            foreach (IInteractable interactable in interactableList)
            {
                // Determine if the current interactable is closer than the previously found one
                if (closestInteractable == null || IsInteractableCloser(interactable, closestInteractable))
                {
                    closestInteractable = interactable;
                }
            }

            return closestInteractable;
        }

        // Compares the distance of two interactable objects to the player.
        private bool IsInteractableCloser(IInteractable interactable, IInteractable closestInteractable)
            => Vector3.Distance(transform.position, interactable.GetTransform().position)
               < Vector3.Distance(transform.position, closestInteractable.GetTransform().position);
    }
}