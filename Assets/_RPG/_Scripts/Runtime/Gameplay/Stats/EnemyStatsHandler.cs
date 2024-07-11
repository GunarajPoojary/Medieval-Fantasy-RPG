using UnityEngine;

namespace RPG.Gameplay.Stats
{
    public class EnemyStatsHandler : MonoBehaviour
    {
        [SerializeField] private GameObject _ragdoll;
        [field: SerializeField] public Characters.CharacterProfile_SO Profile { get; private set; }
        [SerializeField] private Animator _animator;

        public StatModifier RuntimeStats { get; set; }

        private void Awake()
        {
            RuntimeStats = new StatModifier(Profile.Stats);
        }

        public void TakeDamage(float damage)
        {
            _animator.SetTrigger("TakeHit");

            if (RuntimeStats.GetStatValue(StatType.Health) > 0)
            {
                float mitigatedDamage = 0.4f * damage;
                RuntimeStats.ChangeStatValue(StatType.Defense, (int)-damage);
                RuntimeStats.ChangeStatValue(StatType.Health, (int)-mitigatedDamage);
            }
            else
            {
                RuntimeStats.ChangeStatValue(StatType.Health, (int)-damage);
            }

            if (RuntimeStats.GetStatValue(StatType.Health) <= 0)
                Die();
        }

        public void Die()
        {
            Instantiate(_ragdoll, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
