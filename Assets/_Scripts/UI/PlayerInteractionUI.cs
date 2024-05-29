using GunarajCode.Gameplay;
using UnityEngine;

namespace GunarajCode.UI
{
    /// <summary>
    /// Represents the UI for player interactions.
    /// </summary>
    public class PlayerInteractionUI : MonoBehaviour
    {
        [SerializeField] private GameObject _interactionUI;
        [SerializeField] private PlayerInteract _playerInteract;

        private void Awake() => _playerInteract.OnGetInteractable += OnGetInteractable;

        private void OnDestroy()
        {
            if (_playerInteract != null)
                _playerInteract.OnGetInteractable -= OnGetInteractable;
        }

        private void OnGetInteractable(IInteractable interactable)
        {
            if (interactable != null)
                Show();
            else
                Hide();
        }

        private void Show() => _interactionUI.SetActive(true);

        private void Hide() => _interactionUI.SetActive(false);
    }
}
