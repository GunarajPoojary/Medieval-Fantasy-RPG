using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace RPG
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private GameSceneSO _gameplayScene = default;
        [SerializeField] private LoadEventChannelSO _loadLocationChannel = default;
        [SerializeField] private LoadEventChannelSO _loadMenu = default;
        //[SerializeField] private BoolEventChannelSO _toggleLoadingScreen = default;
        [SerializeField] private SceneLoadProgressEventChannelSO _loadingScreen = default;
        private bool _isLoading = false;
        private GameSceneSO _sceneToLoad;
        private GameSceneSO _currentlyLoadedScene;
        private bool _showLoadingScreen;
        private SceneInstance _gameplayManagerSceneInstance;
        private AsyncOperationHandle<SceneInstance> _loadingOperationHandle;
        private AsyncOperationHandle<SceneInstance> _gameplayManagerLoadingOpHandle;

        private void OnEnable()
        {
            _loadMenu.OnLoadingRequested += LoadMenu;
            _loadLocationChannel.OnLoadingRequested += LoadLocation;
        }

        private void OnDisable()
        {
            _loadMenu.OnLoadingRequested -= LoadMenu;
            _loadLocationChannel.OnLoadingRequested -= LoadLocation;
        }

        private void LoadLocation(GameSceneSO locationToLoad, bool showLoadingScreen, bool fadeScreen)
        {
            //Prevent a double-loading, for situations where the player falls in two Exit colliders in one frame
            if (_isLoading)
                return;

            _sceneToLoad = locationToLoad;
            _showLoadingScreen = showLoadingScreen;
            _isLoading = true;

            if (_gameplayManagerSceneInstance.Scene == null || !_gameplayManagerSceneInstance.Scene.isLoaded)
            {
                _gameplayManagerLoadingOpHandle = _gameplayScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true);
                _gameplayManagerLoadingOpHandle.Completed += OnGameplayManagerSceneLoaded;
            }
            else
            {
                UnloadPreviousScene();
            }
        }

        private void UnloadPreviousScene()
        {
            if (_currentlyLoadedScene != null) //would be null if the game was started in Initialisation
            {
                if (_currentlyLoadedScene.SceneReference.OperationHandle.IsValid())
                {
                    //Unload the scene through its AssetReference, i.e. through the Addressable system
                    _currentlyLoadedScene.SceneReference.UnLoadScene();
                }
            }

            LoadNewScene();
        }

        private void OnGameplayManagerSceneLoaded(AsyncOperationHandle<SceneInstance> handle)
        {
            _gameplayManagerSceneInstance = handle.Result;

            UnloadPreviousScene();
        }

        /// <summary>
        /// Prepares to load the main menu scene, first removing the Gameplay scene in case the game is coming back from gameplay to menus.
        /// </summary>
        private void LoadMenu(GameSceneSO menuToLoad, bool showLoadingScreen, bool fadeScreen)
        {
            //Prevent a double-loading, for situations where the player falls in two Exit colliders in one frame
            if (_isLoading)
                return;

            _sceneToLoad = menuToLoad;
            _showLoadingScreen = showLoadingScreen;
            _isLoading = true;

            //In case we are coming from a Location back to the main menu, we need to get rid of the persistent Gameplay manager scene
            if (_gameplayManagerSceneInstance.Scene != null
                && _gameplayManagerSceneInstance.Scene.isLoaded)
                Addressables.UnloadSceneAsync(_gameplayManagerLoadingOpHandle, true);


            UnloadPreviousScene();
        }

        /// <summary>
        /// Kicks off the asynchronous loading of a scene, either menu or Location.
        /// </summary>
        private void LoadNewScene()
        {
            _loadingOperationHandle = _sceneToLoad.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true, 0);

            if (_showLoadingScreen)
                _loadingScreen.RaiseEvent(true, _loadingOperationHandle);

            _loadingOperationHandle.Completed += OnNewSceneLoaded;
        }

        private void OnNewSceneLoaded(AsyncOperationHandle<SceneInstance> handle)
        {
            _currentlyLoadedScene = _sceneToLoad;
            _isLoading = false;

            if (_showLoadingScreen)
                _loadingScreen.RaiseEvent(false, handle);
        }
    }
}