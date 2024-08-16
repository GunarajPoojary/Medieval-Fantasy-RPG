using System;
using UnityEngine;

namespace RPG.Core
{
    /// <summary>
    /// Generic Event Channel that broadcasts an event with no parameters and returns a value of type T.
    /// </summary>
    public class GenericNonVoidNonParameterEventChannelSO<T> : ScriptableObject
    {
        [Tooltip("The action to perform; Listeners subscribe to this Func delegate which takes no parameters and returns a value.")]
        public Func<T> OnEventRaised;

        public T RaiseEvent()
        {
            if (OnEventRaised != null)
            {
                return OnEventRaised.Invoke();
            }
            else
            {
                Debug.LogWarning("No subscribers for event");
                return default;
            }
        }
    }
}
