using Cinemachine;
using UnityEngine;

namespace RPG.Cameras
{
    /// <summary>
    /// Handles camera zoom functionality using Cinemachine's FramingTransposer.
    /// Allows smooth zooming in and out based on player input.
    /// </summary>
    public class CameraZoom : MonoBehaviour
    {
        [SerializeField][Range(0f, 12f)] private float _defaultDistance = 6f;  // Default camera distance from the target.
        [SerializeField][Range(0f, 12f)] private float _minimumDistance = 1f;  // Minimum allowable zoom distance.
        [SerializeField][Range(0f, 12f)] private float _maximumDistance = 6f;  // Maximum allowable zoom distance.

        [SerializeField][Range(0f, 20f)] private float _smoothing = 4f;        // Smoothing factor for zoom transitions.
        [SerializeField][Range(0f, 20f)] private float _zoomSensitivity = 1f;  // Sensitivity of the zoom input.

        private CinemachineFramingTransposer _framingTransposer;  // Reference to the Cinemachine FramingTransposer component.
        private CinemachineInputProvider _inputProvider;          // Reference to the Cinemachine Input Provider for handling player input.

        private float _currentTargetDistance;  // Current target distance for the camera.

        private void Awake()
        {
            // Get references to required components.
            _framingTransposer = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>();
            _inputProvider = GetComponent<CinemachineInputProvider>();

            // Set the initial target distance to the default distance.
            _currentTargetDistance = _defaultDistance;
        }

        // Called every frame to update the camera zoom.
        private void Update() => Zoom();

        /// <summary>
        /// Handles the zoom functionality based on player input.
        /// </summary>
        private void Zoom()
        {
            // Get the zoom input value from the input provider.
            float zoomValue = _inputProvider.GetAxisValue(2) * _zoomSensitivity;

            // Adjust the target distance based on the zoom input and clamp it within the allowed range.
            _currentTargetDistance = Mathf.Clamp(_currentTargetDistance + zoomValue, _minimumDistance, _maximumDistance);

            // Get the current camera distance.
            float currentDistance = _framingTransposer.m_CameraDistance;

            // If the current distance matches the target distance, no need to proceed.
            if (currentDistance == _currentTargetDistance)
            {
                return;
            }

            // Smoothly interpolate the camera distance towards the target distance.
            float lerpedZoomValue = Mathf.Lerp(currentDistance, _currentTargetDistance, _smoothing * Time.deltaTime);

            // Set the new camera distance.
            _framingTransposer.m_CameraDistance = lerpedZoomValue;
        }
    }
}
