using System;
using RPG.UI;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace RPG
{
    public class LoadingBarController : MonoBehaviour
    {
        [SerializeField] private LoadingBar _loadingInterface = default;

        [Header("Listening on")]
        //[SerializeField] private BoolEventChannelSO _toggleLoadingScreen = default;
        [SerializeField] private SceneLoadProgressEventChannelSO _loadingScreen = default;

        private void OnEnable()
        {
            _loadingScreen.OnEventRaised += ToggleLoadingScreen;
        }

        private void OnDisable()
        {
            _loadingScreen.OnEventRaised -= ToggleLoadingScreen;
        }


        private void ToggleLoadingScreen(bool state, AsyncOperationHandle<SceneInstance> opHandle)
        {
            _loadingInterface.gameObject.SetActive(state);

            if (!state) return;
            
            _loadingInterface.ResetBar();
            _loadingInterface.UpdateBar(opHandle.PercentComplete);
        }

    }
}