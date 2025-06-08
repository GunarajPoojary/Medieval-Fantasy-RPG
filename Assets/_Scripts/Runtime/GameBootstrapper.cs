using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

/// <summary>
/// This class is responsible for starting the game by loading the persistent managers scene 
/// and raising the event to load the Main Menu
/// </summary>

public class GameBootstrapper : MonoBehaviour
{
	[SerializeField] private GameSceneSO _managersScene = default;
	[SerializeField] private GameSceneSO _menuToLoad = default;

	[Header("Broadcasting on")]
	[SerializeField] private AssetReference _menuLoadChannel = default;
	private const int BOOTSTRAPPER_SCENE_INDEX = 0;

	private void Start()
	{
		// Load the persistent managers scene
		_managersScene.SceneReference.LoadSceneAsync(LoadSceneMode.Additive, true).Completed += LoadEventChannel;
	}

	// Now the persistent Managers scene is active, let's load the event channel from addressable
	private void LoadEventChannel(AsyncOperationHandle<SceneInstance> obj)
	{
		_menuLoadChannel.LoadAssetAsync<LoadEventChannelSO>().Completed += LoadMainMenu;
	}

	// Once the eventchannel has been loaded from addressable raise the event which will be listened by SceneLoader
	private void LoadMainMenu(AsyncOperationHandle<LoadEventChannelSO> eventChannel)
	{
		eventChannel.Result.RaiseEvent(_menuToLoad, true);

		SceneManager.UnloadSceneAsync(BOOTSTRAPPER_SCENE_INDEX); // Bootstrapper scene is the only scene in BuildSettings, thus it has index 0
	}
}