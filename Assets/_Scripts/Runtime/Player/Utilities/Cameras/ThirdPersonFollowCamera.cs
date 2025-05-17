using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG
{
    public class ThirdPersonFollowCamera : MonoBehaviour
    {
        [SerializeField] private PlayerCameraData _playerCameraData;

        private PlayerInputHandler _playerInput;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;
        private const float THRESHOLD = 0.01f;

        private Vector2 _look;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInputHandler>();
        }

        private void OnEnable()
        {
            _playerInput.LookAction.performed += OnLook;
            _playerInput.LookAction.canceled += OnLook;
        }

        private void OnDisable()
        {
            _playerInput.LookAction.performed -= OnLook;
            _playerInput.LookAction.canceled -= OnLook;
        }

        private void Start()
        {
            _cinemachineTargetYaw = _playerCameraData.CinemachineCameraTarget.rotation.eulerAngles.y;
        }

        private void LateUpdate()
        {
            UpdateCameraRotation();
        }
        
        private void OnLook(InputAction.CallbackContext ctx)
        {
            _look = ctx.ReadValue<Vector2>();
        }

        private void UpdateCameraRotation()
        {
            if (_look.sqrMagnitude >= THRESHOLD)
            {
                _cinemachineTargetYaw += _look.x;
                _cinemachineTargetPitch += _look.y;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _playerCameraData.BottomClamp, _playerCameraData.TopClamp);

            _playerCameraData.CinemachineCameraTarget.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }
    }
}