using UnityEngine.Events;
using UnityEngine;

namespace ProjectEmbersteel.Events.EventChannel
{
    /// <summary>
    /// This class is used for Events that have a Void argument.
    /// </summary>
    [CreateAssetMenu(menuName = "Custom/Events/Void Event Channel")]
    public class VoidEventChannelSO : DescriptionBaseSO
    {
        public event UnityAction OnEventRaised;

        public void RaiseEvent() => OnEventRaised?.Invoke();
    }
}