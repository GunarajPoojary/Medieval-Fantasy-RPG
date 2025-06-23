using UnityEngine;

namespace ProjectEmbersteel.InteractionSystem
{
    [RequireComponent(typeof(BoxCollider))]
    public class InteractionController : MonoBehaviour
    {
        [SerializeField] private LayerMask _interactableLayer;
        [SerializeField] private UIInteraction _uIInteraction;
        private IInteractable _currentInteractable;

        private void OnTriggerEnter(Collider collider)
        {
            if (((1 << collider.gameObject.layer) & _interactableLayer) != 0)
            {
                if (!collider.TryGetComponent(out IInteractable interactable))
                    return;

                _uIInteraction.ToggleUI(true);

                _currentInteractable = interactable;

                _currentInteractable.Interact();
            }
        }

        private void OnTriggerExit(Collider collider)
        {
            _uIInteraction.ToggleUI(false);
        }
    }
}