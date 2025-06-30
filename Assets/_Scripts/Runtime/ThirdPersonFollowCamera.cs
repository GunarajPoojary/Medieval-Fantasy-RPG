using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;

namespace ProjectEmbersteel.Player
{
    public class ThirdPersonFollowCamera : MonoBehaviour, IInputAxisOwner
    {
        [Tooltip("Horizontal Rotation.  Value is in degrees, with 0 being centered.")]
        [SerializeField] private InputAxis _horizontalLook = new() { Range = new Vector2(-180, 180), Wrap = true, Recentering = InputAxis.RecenteringSettings.Default };

        [Tooltip("Vertical Rotation.  Value is in degrees, with 0 being centered.")]
        [SerializeField] private InputAxis _verticalLook = new() { Range = new Vector2(-70, 70), Recentering = InputAxis.RecenteringSettings.Default };

        [SerializeField] private PlayerController _controller;
        
        private Transform _controllerTransform;    // cached for efficiency
        private Quaternion _desiredWorldRotation;

        /// Report the available input axes to the input axis controller.
        /// We use the Input Axis Controller because it works with both the Input package
        /// and the Legacy input system.  This is sample code and we
        /// want it to work everywhere.
        void IInputAxisOwner.GetInputAxes(List<IInputAxisOwner.AxisDescriptor> axes)
        {
            axes.Add(new() { DrivenAxis = () => ref _horizontalLook, Name = "Horizontal Look", Hint = IInputAxisOwner.AxisDescriptor.Hints.X });
            axes.Add(new() { DrivenAxis = () => ref _verticalLook, Name = "Vertical Look", Hint = IInputAxisOwner.AxisDescriptor.Hints.Y });
        }

        private void Awake() => _controllerTransform = _controller.transform;

        private void OnEnable()
        {
            _controller.PreUpdate += UpdatePlayerRotation;
            _controller.PostUpdate += PostUpdate;
        }

        private void OnDisable()
        {
            _controller.PreUpdate -= UpdatePlayerRotation;
            _controller.PostUpdate -= PostUpdate;
        }

        // This is called by the player controller before it updates its own rotation.
        private void UpdatePlayerRotation()
        {
            Transform targetTransform = transform;
            targetTransform.localRotation = Quaternion.Euler(_verticalLook.Value, _horizontalLook.Value, 0);
            _desiredWorldRotation = targetTransform.rotation;
        }

        // Callback for player controller to update our rotation after it has updated its own.
        private void PostUpdate()
        {
            // After player has been rotated, we subtract any rotation change
            // from our own transform, to maintain our world rotation
            transform.rotation = _desiredWorldRotation;
            Vector3 delta = (Quaternion.Inverse(_controllerTransform.rotation) * _desiredWorldRotation).eulerAngles;
            _verticalLook.Value = NormalizeAngle(delta.x);
            _horizontalLook.Value = NormalizeAngle(delta.y);
        }

        private float NormalizeAngle(float angle)
        {
            while (angle > 180)
                angle -= 360;
            while (angle < -180)
                angle += 360;
            return angle;
        }
    }
}