using UnityEngine.Events;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace RPG
{
    /// <summary>
    /// This class is used for Events that have a AsyncOperationHandle argument.
    /// Example: An event to know the scen load porgress for Loading Screen
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Events/Loading Event Channel")]
    public class SceneLoadProgressEventChannelSO : DescriptionBaseSO
    {
        public event UnityAction<bool, AsyncOperationHandle<SceneInstance>> OnEventRaised;

        public void RaiseEvent(bool value, AsyncOperationHandle<SceneInstance> operationHandle) => OnEventRaised?.Invoke(value, operationHandle);
    }
}