using ProjectEmbersteel.DialogueSystem;
using ProjectEmbersteel.Events.EventChannel;
using UnityEngine;

namespace ProjectEmbersteel.UI
{
	public class UIManager : MonoBehaviour
	{
		public static UIManager Instance { get; private set;}

		[SerializeField] private GameObject _interactionPanel;
		[SerializeField] private BoolEventChannelSO _toggleInteractionPanel;

        private void Awake() => Instance = this;

        private void OnEnable() => SubscribeToEvents(true);
        private void OnDisable() => SubscribeToEvents(false);

        private void SubscribeToEvents(bool subscribe)
		{
			if (subscribe)
				_toggleInteractionPanel.OnEventRaised += ToggleInteractionUI;
			else
				_toggleInteractionPanel.OnEventRaised -= ToggleInteractionUI;
		}

		public void ToggleInteractionUI(bool toggle) => _interactionPanel.SetActive(toggle);

		private void OpenUIDialogue(string dialogueLine, Actor actor)
		{
			bool isProtagonistTalking = (actor.ActorType == ActorType.Player);
			// _dialogueController.SetDialogue(dialogueLine, actor, isProtagonistTalking);
			// _interactionPanel.gameObject.SetActive(false);
			// _dialogueController.gameObject.SetActive(true);
		}

		private void CloseUIDialogue(int dialogueType)
		{
			// _selectionHandler.Unselect();
			// _dialogueController.gameObject.SetActive(false);
			// _onInteractionEndedEvent.RaiseEvent();
		}
	}
}