using RPG.Core;
using RPG.ScriptableObjects.Stats;
using UnityEngine;

namespace RPG.Stats
{
    /// <summary>
    /// Handles a character's stats, including taking damage and death, and providing stat values in Gameplay Scene.
    /// </summary>
    public class CharacterStatsHandler : MonoBehaviour, IDamageable, ICharacterStatsProvider
    {
        [SerializeField] private GameObject _ragdoll;
        [SerializeField] private RuntimeStats _runtimeStats;

        private Animator _animator;

        private void Awake() => _animator = GetComponentInChildren<Animator>();

        //private void Update()
        //{
        //    if (Input.GetKeyDown(KeyCode.H))
        //    {
        //        TakeDamage(15f);
        //    }
        //}

        #region IDamageable Methods
        public void TakeDamage(float damage)
        {
            _animator.SetTrigger("TakeHit");

            float health = _runtimeStats.GetCurrentStatValue(StatType.Health);

            if (health > 0)
            {
                float mitigatedDamage = damage * 0.4f;
                _runtimeStats.ChangeCurrentStatValue(StatType.Defense, -(int)damage);
                _runtimeStats.ChangeCurrentStatValue(StatType.Health, -(int)mitigatedDamage);
            }
            else
            {
                _runtimeStats.ChangeCurrentStatValue(StatType.Health, -(int)damage);
            }

            if (_runtimeStats.GetCurrentStatValue(StatType.Health) <= 0)
            {
                Die();
            }
        }

        public void Die()
        {
            Instantiate(_ragdoll, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
        #endregion

        #region ICharacterStatsProvider Methods
        public float GetCurrentHealth() => _runtimeStats.GetCurrentStatValue(StatType.Health);

        public float GetCurrentDefense() => _runtimeStats.GetCurrentStatValue(StatType.Defense);

        public float GetOverallHealth() => _runtimeStats.GetOverallStatValue(StatType.Health);

        public float GetOverallDefense() => _runtimeStats.GetOverallStatValue(StatType.Defense);
        #endregion
    }
}