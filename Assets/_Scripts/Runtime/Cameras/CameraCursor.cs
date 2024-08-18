using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Cameras
{
    public class CameraCursor : MonoBehaviour
    {
        [SerializeField] private InputActionReference _cameraToggleInputAction;
        [SerializeField] private bool _startHidden;

        [SerializeField] private CinemachineInputProvider _inputProvider;
        [SerializeField] private bool _disableCameraLookOnCursorVisible;
        [SerializeField] private bool _disableCameraZoomOnCursorVisible;

        private void Awake()
        {
            _cameraToggleInputAction.action.started += OnCameraCursorToggled;

            if (_startHidden)
            {
                ToggleCursor();
            }
        }

        private void OnEnable() => _cameraToggleInputAction.asset.Enable();

        private void OnDisable() => _cameraToggleInputAction.asset.Disable();

        private void OnCameraCursorToggled(InputAction.CallbackContext context) => ToggleCursor();

        private void ToggleCursor()
        {
            Cursor.visible = !Cursor.visible;

            if (!Cursor.visible)
            {
                Cursor.lockState = CursorLockMode.Locked;

                _inputProvider.enabled = true;

                _inputProvider.XYAxis.action.Enable();
                _inputProvider.ZAxis.action.Enable();

                return;
            }

            Cursor.lockState = CursorLockMode.None;

            _inputProvider.enabled = false;

            if (_disableCameraLookOnCursorVisible)
            {
                _inputProvider.XYAxis.action.Disable();
            }

            if (_disableCameraZoomOnCursorVisible)
            {
                _inputProvider.ZAxis.action.Disable();
            }
        }
    }
}