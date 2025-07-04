using ProjectEmbersteel.DialogueSystem;
using ProjectEmbersteel.Events.EventChannel;
using ProjectEmbersteel.InteractionSystem;
using UnityEngine;

namespace ProjectEmbersteel.Characters
{
    public class NPC : MonoBehaviour, IInteractable
    {
        [SerializeField] private DialogueDataSO _defaultDialogueDataSO;
        [SerializeField] private DialogueEventChannelSO _startDialogueEvent;

        public void Interact()
        {
            _startDialogueEvent.RaiseEvent(_defaultDialogueDataSO);
        }
    }
}