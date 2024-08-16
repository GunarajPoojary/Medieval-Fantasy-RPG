using UnityEngine;
using UnityEngine.Events;

namespace RPG.Core
{
    /// <summary>
    /// Generic Event Channel that broadcasts an event carrying a single parameter of type T and has a void return type.
    /// </summary>
    public class GenericVoidReturnSingleParameterEventChannelSO<T> : ScriptableObject
    {
        [Tooltip("The action to perform; Listeners subscribe to this UnityAction which takes a single parameter of type T.")]
        public UnityAction<T> OnEventRaised;

        public void RaiseEvent(T parameter) => OnEventRaised?.Invoke(parameter);
    }
}