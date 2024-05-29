using UnityEngine;
using UnityEngine.AI;

namespace GunarajCode
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private float _lookRadius = 10f;

        private Transform _target;
        private NavMeshAgent _agent;

        private int _isChasingHash, _isAttackingHash, _speedHash;

        private const string IS_CHASING = "IsChasing";
        private const string IS_ATTACKING = "IsAttacking";
        private const string SPEED = "Speed";
        private float _smoothValue = 5f;

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            InitializeAnimationHashes();
        }

        private void InitializeAnimationHashes()
        {
            _isAttackingHash = Animator.StringToHash(IS_ATTACKING);
            _isChasingHash = Animator.StringToHash(IS_CHASING);
            _speedHash = Animator.StringToHash(SPEED);
        }

        private void Start()
        {
            _target = PlayerManager.Instance.Player.transform;
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
                }
            }
            else
            {
                _agent.ResetPath();
            }
        }

        private void FaceTarget()
        {
            Vector3 direction = (_target.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * _smoothValue);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _lookRadius);
        }
    }
}
