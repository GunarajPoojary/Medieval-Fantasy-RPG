using UnityEngine.Events;
using UnityEngine;
using ProjectEmbersteel.InteractionSystem;
using ProjectEmbersteel.DialogueSystem;

namespace ProjectEmbersteel.Events.EventChannel
{
    /// <summary>
    /// This class is used for Events that have a DialogueDataSO argument.
    /// Example: An event to start Dialogue
    /// </summary>
    [CreateAssetMenu(fileName = "DialogueEventChannel", menuName = "Custom/Events/Dialogue Event Channel")]
    public class DialogueEventChannelSO : DescriptionBaseSO
    {
        public event UnityAction<DialogueDataSO> OnEventRaised;

        public void RaiseEvent(DialogueDataSO dialogue) => OnEventRaised?.Invoke(dialogue);
    }
}