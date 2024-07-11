using RPG.Core;
using RPG.SaveLoad;
using UnityEngine;

namespace RPG.Gameplay.Stats
{
    public class PlayerStatsHandler : MonoBehaviour, ISaveable
    {
        [field: SerializeField] public Characters.CharacterProfile_SO Profile { get; private set; }
        [SerializeField] private Animator _animator;

        public StatModifier RuntimeStats { get; private set; }

        private void Awake() => RuntimeStats = new(Profile.Stats);

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.H))
                TakeDamage(15f);
        }

        public void TakeDamage(float damage)
        {
            _animator.SetTrigger("TakeHit");

            float health = RuntimeStats.GetStatValue(StatType.Health);
            if (health > 0)
            {
                float mitigatedDamage = damage * 0.4f;
                RuntimeStats.ChangeStatValue(StatType.Defense, -(int)damage);
                RuntimeStats.ChangeStatValue(StatType.Health, -(int)mitigatedDamage);
            }
            else
            {
                RuntimeStats.ChangeStatValue(StatType.Health, -(int)damage);
            }

            if (RuntimeStats.GetStatValue(StatType.Health) <= 0)
                Die();
        }

        public virtual void Die() => Debug.Log($"{transform.name} died");

        public void LoadData(GameData data)
        {
            if (GameManager.Instance.IsFirstDataLoad)
                NewGameLoadData(data);
            else
                RuntimeStats.RuntimeStatTypeToValueMap = data.PlayerStats;

            transform.position = data.Position;
            transform.rotation = data.Rotation;
        }

        private void NewGameLoadData(GameData data)
        {
            for (int i = 0; i < data.PlayerStats.Count; i++)
                RuntimeStats.ChangeStatValue((StatType)i, 0);

            GameManager.Instance.IsFirstDataLoad = false;
        }

        public void SaveData(GameData data)
        {
            data.PlayerStats = RuntimeStats.RuntimeStatTypeToValueMap;
            data.Position = transform.position;
            data.Rotation = transform.rotation;
        }
    }
}
