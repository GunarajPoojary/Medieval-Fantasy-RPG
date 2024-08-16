using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Cameras
{
    /// <summary>
    /// Manages the visibility and locking state of the cursor.
    /// Toggles camera input based on cursor visibility and handles specific cases depending on the Cinemachine version.
    /// </summary>
    public class CameraCursor : MonoBehaviour
    {
        [SerializeField] private InputActionReference _cameraToggleInputAction;  // Input action to toggle the cursor visibility.
        [SerializeField] private bool _startHidden;                             // Should the cursor start hidden?

        [SerializeField] private CinemachineInputProvider _inputProvider;       // Cinemachine Input Provider for handling camera look and zoom.
        [SerializeField] private bool _disableCameraLookOnCursorVisible;        // Should camera look be disabled when the cursor is visible?
        [SerializeField] private bool _disableCameraZoomOnCursorVisible;        // Should camera zoom be disabled when the cursor is visible?

        [Tooltip("If you're using Cinemachine 2.8.4 or under, untick this option.\nIf unticked, both Look and Zoom will be disabled.")]
        [SerializeField] private bool _fixedCinemachineVersion;                 // Indicates whether to use specific handling for different Cinemachine versions.

        private void Awake()
        {
            // Subscribe to the input action for toggling the cursor visibility.
            _cameraToggleInputAction.action.started += OnCameraCursorToggled;

            // If starting with the cursor hidden, toggle its state.
            if (_startHidden)
            {
                ToggleCursor();
            }
        }

        private void OnEnable() => _cameraToggleInputAction.asset.Enable();  // Enable the input action when the script is enabled.

        private void OnDisable() => _cameraToggleInputAction.asset.Disable();  // Disable the input action when the script is disabled.

        /// <summary>
        /// Callback for when the camera cursor toggle input is activated.
        /// </summary>
        /// <param name="context">Input action context.</param>
        private void OnCameraCursorToggled(InputAction.CallbackContext context) => ToggleCursor();

        /// <summary>
        /// Toggles the visibility and lock state of the cursor, and adjusts camera input accordingly.
        /// </summary>
        private void ToggleCursor()
        {
            Cursor.visible = !Cursor.visible;  // Toggle cursor visibility.

            if (!Cursor.visible)
            {
                // Lock the cursor to the center of the screen.
                Cursor.lockState = CursorLockMode.Locked;

                if (!_fixedCinemachineVersion)
                {
                    // Enable the input provider for older versions of Cinemachine.
                    _inputProvider.enabled = true;

                    return;
                }

                // Enable look and zoom input for newer versions of Cinemachine.
                _inputProvider.XYAxis.action.Enable();
                _inputProvider.ZAxis.action.Enable();

                return;
            }

            // Unlock the cursor when it's visible.
            Cursor.lockState = CursorLockMode.None;

            if (!_fixedCinemachineVersion)
            {
                // Disable the input provider for older versions of Cinemachine.
                _inputProvider.enabled = false;

                return;
            }

            // Disable look input if configured to do so when the cursor is visible.
            if (_disableCameraLookOnCursorVisible)
            {
                _inputProvider.XYAxis.action.Disable();
            }

            // Disable zoom input if configured to do so when the cursor is visible.
            if (_disableCameraZoomOnCursorVisible)
            {
                _inputProvider.ZAxis.action.Disable();
            }
        }
    }
}