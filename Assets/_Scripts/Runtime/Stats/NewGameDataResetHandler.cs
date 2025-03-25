using UnityEngine;

namespace RPG
{
    /// <summary>
    /// Handles resetting game data when starting a new game, particularly resetting the player's stats.
    /// </summary>
    public class NewGameDataResetHandler : MonoBehaviour
    {
        //[SerializeField] private VoidReturnNonParameterEventChannelSO _newGameChannelSO;  // Event channel for new game trigger
        [Space]
        [SerializeField] private RuntimeStats _runtimeStats;
        [Space]
        [SerializeField] private BaseStats _playerBaseStats;

        private void OnEnable()
        {
            //_newGameChannelSO.OnEventRaised += HandleNewGame;
        }

        private void OnDisable()
        {
            //_newGameChannelSO.OnEventRaised -= HandleNewGame;
        }

        private void HandleNewGame()
        {
            _runtimeStats.ResetCurrentStats(_playerBaseStats.GetAllStats());
            _runtimeStats.ResetOverallStats(_playerBaseStats.GetAllStats());
        }
    }
}