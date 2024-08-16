using UnityEngine;

namespace RPG.Gameplay.World.UI
{
    public class PlayerInteractionUI : MonoBehaviour
    {
        [SerializeField] private GameObject _interactionUI;
        [SerializeField] private Interact _playerInteract;

        private void Awake() => _playerInteract.OnGetInteractable += OnGetInteractable;

        private void OnDestroy()
        {
            if (_playerInteract != null)
            {
                _playerInteract.OnGetInteractable -= OnGetInteractable;
            }
        }

        private void OnGetInteractable(IInteractable interactable) => _interactionUI.SetActive(interactable != null);
    }
}