using ProjectEmbersteel.Events.EventChannel;
using ProjectEmbersteel.SceneManagement;
using UnityEngine;

namespace ProjectEmbersteel.UI
{
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField] private GameSceneSO _locationToLoad;
        [SerializeField] private LoadSceneEventChannelSO _loadLocationChannel;

        public void StartNewGame()
        {
            _loadLocationChannel.RaiseEvent(_locationToLoad, true, false);
        }
    }
}