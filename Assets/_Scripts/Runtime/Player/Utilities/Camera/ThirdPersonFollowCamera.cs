using ProjectEmbersteel.Utilities.Inputs.ScriptableObjects;
using UnityEngine;

namespace ProjectEmbersteel
{
    public class ThirdPersonFollowCamera : MonoBehaviour
    {
        [SerializeField] private InputReader _playerInput;
        [SerializeField] private float _sensitivity = 1f;
        [SerializeField] private float _topClamp = 70f;
        [SerializeField] private float _bottomClamp = -30f;
        [SerializeField] private float _rotationLerpSpeed = 5f;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private const float THRESHOLD = 0.01f;

        private Vector2 _look;

        private void OnEnable() => _playerInput.LookAction += OnLook;

        private void OnDisable() => _playerInput.LookAction -= OnLook;

        private void OnLook(Vector2 look) => _look = look;

        private void Start()
        {
            _cinemachineTargetYaw = transform.rotation.eulerAngles.y;
            _cinemachineTargetPitch = transform.rotation.eulerAngles.x;
            
            // Normalize initial pitch to handle cases where it might be > 180
            if (_cinemachineTargetPitch > 180f)
                _cinemachineTargetPitch -= 360f;
        }

        private void LateUpdate() => UpdateCameraRotation();

        private void UpdateCameraRotation()
        {
            if (_look.sqrMagnitude >= THRESHOLD)
            {
                float deltaTimeMultiplier = Time.deltaTime;
                _cinemachineTargetYaw += _look.x * _sensitivity * deltaTimeMultiplier;
                _cinemachineTargetPitch += _look.y * _sensitivity * deltaTimeMultiplier;
            }

            // Properly normalize yaw to prevent accumulation issues
            _cinemachineTargetYaw = NormalizeAngle(_cinemachineTargetYaw);
            _cinemachineTargetPitch = Mathf.Clamp(_cinemachineTargetPitch, _bottomClamp, _topClamp);

            Quaternion targetRotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, _rotationLerpSpeed * Time.deltaTime);
        }

        private static float NormalizeAngle(float angle)
        {
            while (angle > 180f) angle -= 360f;
            while (angle < -180f) angle += 360f;
            return angle;
        }
    }
}