using ProjectEmbersteel.Events.EventChannel;
using ProjectEmbersteel.Utilities.Inputs.ScriptableObjects;
using UnityEngine;

namespace ProjectEmbersteel.InteractionSystem
{
    public class InteractionManager : MonoBehaviour
    {
        [Header("Listening")]
        [SerializeField] private IInteractableEventChannelSO _OnTriggerInteractable;
		[SerializeField] private BoolEventChannelSO _toggleInteractionUI;

        [SerializeField] private InputReader _inputReader;

        private InteractionController _controller;

        private void Awake() => _controller = new(_inputReader, _toggleInteractionUI);

        private void OnEnable() => SubscribeToEvents(true);

        private void OnDisable() => SubscribeToEvents(false);

        private void SubscribeToEvents(bool subscribe)
        {
            if (subscribe)
            {
                _OnTriggerInteractable.OnEventRaised += ToggleUI;
                _controller.AddInputActionCallback();
            }
            else
            {
                _OnTriggerInteractable.OnEventRaised -= ToggleUI;
                _controller.RemoveInputActionCallback();
            }
        }

        private void ToggleUI(bool toggle, IInteractable interactable)
        {
            if (toggle)
                _controller.AddInteractable(interactable);
            else
                _controller.RemoveInteractable(interactable);
        }
    }
}