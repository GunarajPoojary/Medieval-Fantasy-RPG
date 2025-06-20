using UnityEngine.Events;
using UnityEngine;

namespace RPG.Events.EventChannel
{
    /// <summary>
    /// This class is used for Events that have a bool argument.
    /// Example: An event to toggle a UI interface
    /// </summary>
    [CreateAssetMenu(menuName = "Custom/Events/Bool Event Channel")]
    public class BoolEventChannelSO : DescriptionBaseSO
    {
        public event UnityAction<bool> OnEventRaised;

        public void RaiseEvent(bool value) => OnEventRaised?.Invoke(value);
    }
}