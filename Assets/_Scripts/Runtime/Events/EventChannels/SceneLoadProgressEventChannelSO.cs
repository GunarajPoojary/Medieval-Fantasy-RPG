using UnityEngine.Events;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace ProjectEmbersteel.Events.EventChannel
{
    /// <summary>
    /// This class is used for Events that have a AsyncOperationHandle argument.
    /// Example: An event to know the scene load porgress for Loading Screen
    /// </summary>
    [CreateAssetMenu(fileName = "SceneLoadProgressChannel", menuName = "Custom/Events/Scene Load Progress Event Channel")]
    public class SceneLoadProgressEventChannelSO : DescriptionBaseSO
    {
        public event UnityAction<bool, AsyncOperationHandle<SceneInstance>> OnEventRaised;

        public void RaiseEvent(bool value, AsyncOperationHandle<SceneInstance> operationHandle) => OnEventRaised?.Invoke(value, operationHandle);
    }
}