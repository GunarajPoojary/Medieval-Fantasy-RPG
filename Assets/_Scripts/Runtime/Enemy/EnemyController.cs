using UnityEngine;
using UnityEngine.AI;

namespace RPG.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float _lookRadius = 10f;
        [SerializeField] private float _attackCooldown = 3f;
        [SerializeField] private float _smoothValue = 5f;

        private const string ATTACK = "Attack";
        private const string SPEED = "Speed";

        private Animator _animator;
        private Transform _target;
        private NavMeshAgent _agent;

        private float _lastAttackTime;
        private int _attackHash, _speedHash;

        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            _agent = GetComponent<NavMeshAgent>();

            InitializeAnimationHashes();
        }

        private void Start()
        {
            _target = Player.PlayerManager.Instance.Player.transform;
            _lastAttackTime = -_attackCooldown;
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
                        SetAttackAnimation();
                        _lastAttackTime = Time.time; // Update the last attack time
                    }
                }
            }
            else
            {
                _agent.ResetPath();
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _lookRadius);
        }

        private void InitializeAnimationHashes()
        {
            _attackHash = Animator.StringToHash(ATTACK);
            _speedHash = Animator.StringToHash(SPEED);
        }

        private void FaceTarget()
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _smoothValue);
        }

        private void SetAttackAnimation() => _animator.SetTrigger(_attackHash);
    }
}
