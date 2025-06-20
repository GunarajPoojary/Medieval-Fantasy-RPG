using RPG.Events.EventChannel;
using RPG.SceneManagement;
using UnityEngine;

namespace RPG.UI
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