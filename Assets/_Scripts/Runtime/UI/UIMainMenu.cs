using UnityEngine;

namespace RPG
{
    public class UIMainMenu : MonoBehaviour
    {
        [SerializeField] private GameSceneSO _locationToLoad;
        [SerializeField] private LoadEventChannelSO _loadLocationChannel;

        public void StartNewGame()
        {
            _loadLocationChannel.RaiseEvent(_locationToLoad, true, false);
        }
    }
}