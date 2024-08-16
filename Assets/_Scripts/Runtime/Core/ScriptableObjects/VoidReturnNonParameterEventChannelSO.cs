using UnityEngine;
using UnityEngine.Events;

namespace RPG.Core
{
    /// <summary>
    /// Event Channel that broadcasts events without any parameters and has a void return type.
    /// </summary>
    [CreateAssetMenu(menuName = "Events/Void Return Type/Non-Parameter/Void Return Non-Parameter Event Channel")]
    public class VoidReturnNonParameterEventChannelSO : ScriptableObject
    {
        [Tooltip("The action to perform; Listeners subscribe to this UnityAction which takes no parameters.")]
        public event UnityAction OnEventRaised;

        public void RaiseEvent() => OnEventRaised?.Invoke();
    }
}