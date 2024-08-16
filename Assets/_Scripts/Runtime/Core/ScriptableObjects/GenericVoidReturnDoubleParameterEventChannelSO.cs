using UnityEngine;
using UnityEngine.Events;

namespace RPG.Core
{
    /// <summary>
    /// Generic Event Channel that broadcasts an event with two parameters of types T and K, without returning any value.
    /// </summary>
    public class GenericVoidReturnDoubleParameterEventChannelSO<T, K> : ScriptableObject
    {
        [Tooltip("The action to perform; Listeners subscribe to this UnityAction which takes parameters of types T and K.")]
        public UnityAction<T, K> OnEventRaised;

        public void RaiseEvent(T tParameter, K kParameter) => OnEventRaised?.Invoke(tParameter, kParameter);
    }
}