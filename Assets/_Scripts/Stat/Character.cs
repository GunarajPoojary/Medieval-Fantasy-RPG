using UnityEngine;

namespace GunarajCode.Stat
{
    /// <summary>
    /// Represents the character's base statistics and handles stat-related functionality.
    /// </summary>
    public class Character : MonoBehaviour
    {
        public CharacterProfile _profile;
        [SerializeField] private Animator _animator;
        protected int _maxHealth;
        protected int _maxDefense;

        private float _currentDefense, _currentHealth;

        [SerializeField] protected Stats _baseStats;

        public Stats BaseStats { get => _baseStats; }
        public CharacterProfile Profile { get => _profile; }
        public float CurrentHealth { get => _currentHealth; set => _currentHealth = value; }
        public float CurrentDefense { get => _currentDefense; set => _currentDefense = value; }

        protected virtual void Awake()
        {
            _maxHealth = _baseStats.GetStatValue(StatType.Health);
            _maxDefense = _baseStats.GetStatValue(StatType.Defense);
            _currentDefense = _maxDefense;
            _currentHealth = _maxHealth;
        }

        public void TakeDamage(float damage)
        {
            _animator.SetTrigger("TakeHit");
            if (_currentDefense > 0)
            {
                // Mitigate 60% of incoming damage if there is defense
                float mitigatedDamage = 0.4f * damage;
                _currentDefense -= damage; // Apply full damage to defense
                _currentHealth -= mitigatedDamage; // Apply mitigated damage to health
            }
            else
            {
                _currentHealth -= damage; // Apply full damage to health if no defense
            }

            if (_currentHealth <= 0)
                Die();
        }

        public virtual void Die()
        {
            Debug.Log(transform.name + " died");
        }
    }
}