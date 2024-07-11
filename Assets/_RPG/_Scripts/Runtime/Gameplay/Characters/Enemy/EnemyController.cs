using UnityEngine;
using UnityEngine.AI;

namespace RPG.Gameplay.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        private Animator _animator;
        [SerializeField] private float _lookRadius = 10f;
        [SerializeField] private float _attackCooldown = 3f; // Cooldown time between attacks
        [SerializeField] private float _smoothValue = 5f;

        private Transform _target;
        private NavMeshAgent _agent;
        private float _lastAttackTime;

        private int _attackHash, _speedHash;

        private const string ATTACK = "Attack";
        private const string SPEED = "Speed";

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _agent = GetComponent<NavMeshAgent>();
            InitializeAnimationHashes();
        }

        private void InitializeAnimationHashes()
        {
            _attackHash = Animator.StringToHash(ATTACK);
            _speedHash = Animator.StringToHash(SPEED);
        }

        private void Start()
        {
            _target = Player.PlayerManager.Instance.Player.transform;
            _lastAttackTime = -_attackCooldown; // Ensure the enemy can attack immediately at the start
        }

        private void Update()
        {
            _animator.SetFloat(_speedHash, _agent.velocity.magnitude);

            float distance = Vector3.Distance(_target.position, transform.position);
            bool isWithinLookRadius = distance <= _lookRadius;

            if (isWithinLookRadius)
            {
                _agent.destination = _target.position;
                bool isWithinStoppingDistance = distance <= _agent.stoppingDistance;

                if (isWithinStoppingDistance)
                {
                    FaceTarget();

                    if (Time.time >= _lastAttackTime + _attackCooldown)
                    {
                        AttackTarget();
                        _lastAttackTime = Time.time; // Update the last attack time
                    }
                }
            }
            else
                _agent.ResetPath();
        }

        private void FaceTarget()
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _smoothValue);
        }

        private void AttackTarget()
        {
            _animator.SetTrigger(_attackHash);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _lookRadius);
        }
    }
}
