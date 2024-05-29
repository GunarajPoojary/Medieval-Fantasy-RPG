using UnityEngine;

namespace GunarajCode.Stat
{
    /// <summary>
    /// Represents the character's base statistics and handles stat-related functionality.
    /// </summary>
    public class CharacterStat : MonoBehaviour
    {
        [SerializeField] private int MaxHealth = 100;
        public int CurrentHealth { get; private set; }

        [SerializeField] protected Stats _characterStats;

        private const KeyCode _increaseHealthKey = KeyCode.T;
        private const KeyCode _printStatsKey = KeyCode.H;

        private void Awake() => CurrentHealth = MaxHealth;

        private void Update()
        {
            if (Input.GetKeyDown(_increaseHealthKey))
                _characterStats.ChangeStatValue(StatType.Health, 50);

            if (Input.GetKeyDown(_printStatsKey))
            {
                foreach (var item in _characterStats.StatTypeToValueMap)
                    Debug.Log($"Player's {item.Key} is {item.Value}");
            }
        }

        public void TakeDamage(int damage)
        {
            damage -= _characterStats.GetStatValue(StatType.Defense);
            damage = Mathf.Clamp(damage, 0, int.MaxValue);

            CurrentHealth -= damage;
            Debug.Log(transform.name + " takes " + damage + " damage.");

            if (CurrentHealth <= 0)
                Die();
        }

        public virtual void Die()
        {
            Debug.Log(transform.name + " died ");
        }
    }
}
