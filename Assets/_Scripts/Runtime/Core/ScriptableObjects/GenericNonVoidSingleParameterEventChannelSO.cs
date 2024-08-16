using System;
using UnityEngine;

namespace RPG.Core
{
    /// <summary>
    /// Generic Event Channel that broadcasts an event with a single parameter of type T and returns a value of type K.
    /// </summary>
    public class GenericNonVoidReturnSingleParameterEventChannelSO<T, K> : ScriptableObject
    {
        [Tooltip("The action to perform; Listeners subscribe to this Func delegate which takes a parameter of type T and returns a value of type K.")]
        public Func<T, K> OnEventRaised;

        public K RaiseEvent(T parameter)
        {
            if (OnEventRaised != null)
            {
                return OnEventRaised.Invoke(parameter);
            }
            else
            {
                Debug.LogWarning($"No subscribers for event with parameter: {parameter}");
                return default;
            }
        }
    }
}