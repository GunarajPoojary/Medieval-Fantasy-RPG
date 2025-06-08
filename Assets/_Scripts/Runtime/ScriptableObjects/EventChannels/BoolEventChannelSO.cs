using UnityEngine.Events;
using UnityEngine;

namespace RPG
{
    /// <summary>
    /// This class is used for Events that have a bool argument.
    /// Example: An event to toggle a UI interface
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Events/Bool Event Channel")]
    public class BoolEventChannelSO : DescriptionBaseSO
    {
        public event UnityAction<bool> OnEventRaised;

        public void RaiseEvent(bool value) => OnEventRaised?.Invoke(value);
    }
}