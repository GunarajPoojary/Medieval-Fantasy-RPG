using GunarajCode.Stat;
using UnityEngine;

namespace GunarajCode
{
    public class DamageDealer : MonoBehaviour
    {
        private bool _canDealDamage;
        [SerializeField] private LayerMask _enemyLayerMask;
        private Stats _playerStats;
        [SerializeField] private float _weaponLength;
        [SerializeField] private float _damageMultiplier;

        private void OnEnable()
        {
            _damageMultiplier = Mathf.Clamp(_damageMultiplier, 1.0f, 50.0f);
        }

        private void Awake()
        {
            _playerStats = GetComponentInParent<Player>().BaseStats;
        }

        void Start()
        {
            _canDealDamage = false;
        }

        private int WeaponDamage() => _playerStats.GetStatValue(StatType.Attack);

        private void Update()
        {
            if (_canDealDamage)
            {
                if (Physics.Raycast(transform.position, transform.up, out RaycastHit hit, _weaponLength, _enemyLayerMask))
                {
                    if (hit.transform.TryGetComponent(out Enemy enemy))
                    {
                        enemy.TakeDamage(WeaponDamage() * _damageMultiplier * Time.deltaTime);
                    }
                }
            }
        }

        public void ApplyDamage()
        {
            _canDealDamage = true;
        }

        public void EndDamage()
        {
            _canDealDamage = false;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + transform.up * _weaponLength);
        }
    }
}
