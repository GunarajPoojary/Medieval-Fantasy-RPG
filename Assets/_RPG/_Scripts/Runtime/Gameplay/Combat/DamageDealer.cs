using RPG.Core;
using UnityEngine;

namespace RPG.Gameplay.Combat
{
    public class DamageDealer : MonoBehaviour
    {
        private bool _canDealDamage;
        [SerializeField] private LayerMask _enemyLayerMask;

        private int weaponDamage = 0;
        [SerializeField] private float _weaponLength;
        [SerializeField] private float _damageMultiplier;

        private void Awake()
        {
            if (ViewManager.Instance.GetActiveScene() == 2)
                Destroy(gameObject);
            else if (ViewManager.Instance.GetActiveScene() == 1)
                weaponDamage = GetComponentInParent<Stats.PlayerStatsHandler>().RuntimeStats.GetStatValue(Stats.StatType.Attack);
        }

        private void Start()
        {
            _damageMultiplier = Mathf.Clamp(_damageMultiplier, 1.0f, 50.0f);

            _canDealDamage = false;
        }

        private void Update()
        {
            if (_canDealDamage)
            {
                if (Physics.Raycast(transform.position, transform.up, out RaycastHit hit, _weaponLength, _enemyLayerMask))
                {
                    if (hit.transform.TryGetComponent(out Stats.EnemyStatsHandler enemy))
                    {
                        enemy.TakeDamage(weaponDamage * _damageMultiplier * Time.deltaTime);
                    }
                }
            }
        }

        public void ApplyDamage() => _canDealDamage = true;

        public void EndDamage() => _canDealDamage = false;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * _weaponLength);
        }
    }
}
