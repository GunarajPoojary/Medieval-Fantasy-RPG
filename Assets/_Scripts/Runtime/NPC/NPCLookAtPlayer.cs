using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace RPG.NPC
{
    public class NPCLookAtPlayer : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _detectionRadius = 5f;
        [SerializeField] private float _rotationSpeed = 2f;
        [SerializeField] private float _maxHeadRotationAngle = 70f;
        [SerializeField] private float _returnSpeed = 1f; // Speed at which the head returns to its original position

        private Rig _headRig;
        private MultiAimConstraint _aimConstraint;
        private bool _isPlayerInRange;

        private Quaternion _initialRotation;

        private void Awake()
        {
            _headRig = GetComponent<Rig>();
            _aimConstraint = GetComponentInChildren<MultiAimConstraint>();
            _initialRotation = _aimConstraint.data.constrainedObject.rotation;
            _headRig.weight = 0;
        }

        private void Update()
        {
            Transform playerTransform = _aimConstraint.data.sourceObjects[0].transform;
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            if (distanceToPlayer <= _detectionRadius)
            {
                if (!_isPlayerInRange)
                {
                    _isPlayerInRange = true;
                    StopAllCoroutines(); // Stop any ongoing coroutines
                    StartCoroutine(TransitionToLookAtPlayer());
                }
            }
            else if (_isPlayerInRange)
            {
                _isPlayerInRange = false;
                StopAllCoroutines(); // Stop any ongoing coroutines
                StartCoroutine(TransitionToInitialRotation());
            }
        }

        private IEnumerator TransitionToLookAtPlayer()
        {
            while (_headRig.weight < 1f)
            {
                _headRig.weight = Mathf.MoveTowards(_headRig.weight, 1f, Time.deltaTime * _rotationSpeed);
                yield return null;
            }

            Vector3 directionToPlayer = (_aimConstraint.data.sourceObjects[0].transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(directionToPlayer);

            // Clamp rotation
            float angle = Quaternion.Angle(_aimConstraint.data.constrainedObject.rotation, lookRotation);
            if (angle > _maxHeadRotationAngle)
            {
                lookRotation = Quaternion.Slerp(_aimConstraint.data.constrainedObject.rotation, lookRotation, _maxHeadRotationAngle / angle);
            }

            while (Quaternion.Angle(_aimConstraint.data.constrainedObject.rotation, lookRotation) > 0.1f)
            {
                _aimConstraint.data.constrainedObject.rotation = Quaternion.Slerp(_aimConstraint.data.constrainedObject.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
                yield return null;
            }
        }

        private IEnumerator TransitionToInitialRotation()
        {
            while (Quaternion.Angle(_aimConstraint.data.constrainedObject.rotation, _initialRotation) > 0.1f || _headRig.weight > 0f)
            {
                _aimConstraint.data.constrainedObject.rotation = Quaternion.Slerp(_aimConstraint.data.constrainedObject.rotation, _initialRotation, Time.deltaTime * _returnSpeed);
                _headRig.weight = Mathf.MoveTowards(_headRig.weight, 0f, Time.deltaTime * _returnSpeed);
                yield return null;
            }

            _headRig.weight = 0f;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        }
    }
}
