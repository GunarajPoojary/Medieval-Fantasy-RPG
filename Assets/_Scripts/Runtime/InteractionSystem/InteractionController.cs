using ProjectEmbersteel.Events.EventChannel;
using ProjectEmbersteel.Utilities.Inputs.ScriptableObjects;

namespace ProjectEmbersteel.InteractionSystem
{
    public class InteractionController
    {
        private readonly InputReader _inputReader;
        private readonly BoolEventChannelSO _toggleViewChannel;
        private IInteractable _currentInteractable;

        public InteractionController(InputReader inputReader, BoolEventChannelSO toggleViewChannel)
        {
            _inputReader = inputReader;
            _inputReader.DisableActionFor(InputActionType.Interact);
            _toggleViewChannel = toggleViewChannel;
        }

        public void AddInputActionCallback() => _inputReader.InteractPerformedAction += Interact;
        public void RemoveInputActionCallback() => _inputReader.InteractPerformedAction -= Interact;

        public void AddInteractable(IInteractable interactable)
        {
            _currentInteractable = interactable;
            _inputReader.EnableActionFor(InputActionType.Interact);
            _toggleViewChannel.RaiseEvent(true);
        }

        public void RemoveInteractable(IInteractable interactable)
        {
            if (interactable == _currentInteractable)
            {
                _currentInteractable = null;
                _inputReader.DisableActionFor(InputActionType.Interact);
                _toggleViewChannel.RaiseEvent(false);
            }
        }

        private void Interact()
        {
            if (_currentInteractable != null)
            {
                _currentInteractable.Interact();
                RemoveInteractable(_currentInteractable);
            }
        }
    }
}