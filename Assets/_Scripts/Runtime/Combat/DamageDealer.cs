using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG
{
    /// <summary>
    /// Responsible for dealing damage to enemies within a certain range.
    /// </summary>
    public class DamageDealer : MonoBehaviour
    {
        [SerializeField] private LayerMask _enemyLayerMask;
        [SerializeField] private float _weaponLength;
        [SerializeField] private float _damageMultiplier;

        private int _weaponDamage = 0;
        private bool _canDealDamage;

        private void Awake()
        {
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                Destroy(gameObject);
            }
            else if (SceneManager.GetActiveScene().buildIndex == 1)
            {
                return;
                // _weaponDamage = GetComponentInParent<CharacterStatsHandler>().RuntimeStats.GetStatValue(StatType.Attack);
            }
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
                    if (hit.transform.TryGetComponent(out CharacterStatsHandler enemy))
                    {
                        enemy.TakeDamage(_weaponDamage * _damageMultiplier * Time.deltaTime);
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