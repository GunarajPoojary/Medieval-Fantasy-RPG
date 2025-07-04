using ProjectEmbersteel.StatSystem;
using UnityEngine;
using UnityEngine.Events;

namespace ProjectEmbersteel.Events.EventChannel
{
    [CreateAssetMenu(fileName = "StatUpdateEventChannel", menuName = "Custom/Events/Stat Update Event Channel")]
    public class StatUpdateEventChannelSO : DescriptionBaseSO
    {
        public event UnityAction<StatType, Stat> OnEventRaised;

        public void RaiseEvent(StatType type, Stat stat) => OnEventRaised?.Invoke(type, stat);
    }
}