using ProjectEmbersteel.Events.EventChannel;
using UnityEngine;

namespace ProjectEmbersteel.InteractionSystem
{
    [RequireComponent(typeof(BoxCollider))]
    public class Interactor : MonoBehaviour
    {
        [SerializeField] private LayerMask _interactableLayer;
        
        [Header("Broadcasting On")]
        [SerializeField] private IInteractableEventChannelSO _OnTriggerInteractable;

        private void OnTriggerEnter(Collider collider) => HandleInteraction(collider, true);

        private void OnTriggerExit(Collider collider) => HandleInteraction(collider, false);

        private void HandleInteraction(Collider collider, bool isEntering)
        {
            if (((1 << collider.gameObject.layer) & _interactableLayer) == 0)
                return;

            if (!collider.TryGetComponent(out IInteractable interactable))
                return;

            _OnTriggerInteractable.RaiseEvent(isEntering, interactable);
        }
    }
}