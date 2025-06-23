using RPG.CombatSystem;
using RPG.Events.EventChannel;
using RPG.StatSystem;
using UnityEngine;

namespace RPG.Player
{
    /// <summary>
    /// Example usage class demonstrating how to use the stats system.
    /// This would typically be attached to a character GameObject.
    /// </summary>
    public class Player : MonoBehaviour, IDamageable
    {
        [SerializeField] private CharacterReadonlyBaseStatsSO _baseStatsSO;

        [Header("Broadcasting On")]
        [SerializeField] private StatUpdateEventChannel _statUpdateEventChannel;
        [SerializeField] private RuntimeStatUpdateEventChannel _runtimeStatUpdateEventChannel;
        private PlayerStats _playerStats;
        public IStatModifiable StatModifiable { get => _playerStats; }

        private void Awake() => InitializeStats();

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                TakeDamage(30);
            }
        }

        public void TakeDamage(float damage) => _playerStats.HandleDamage(damage);

        private void InitializeStats() => _playerStats = new PlayerStats(_baseStatsSO, HandleStatChanged, HandleRuntimeStatChanged);
        private void HandleStatChanged(StatType statType, Stat stat) => _statUpdateEventChannel.RaiseEvent(statType, stat);
        private void HandleRuntimeStatChanged(StatType statType, float currentValue, float maxValue) => _runtimeStatUpdateEventChannel?.RaiseEvent(statType, currentValue, maxValue);
    }
}