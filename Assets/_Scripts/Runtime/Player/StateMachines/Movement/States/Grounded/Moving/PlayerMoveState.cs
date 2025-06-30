using UnityEngine;

namespace ProjectEmbersteel.Player.StateMachines.Movement.States
{
    /// <summary>
    /// Abstract base class for grounded movement states (Walk, Run).
    /// Handles movement direction, velocity smoothing, and rotation based on input and camera orientation.
    /// </summary>
    public class PlayerMoveState : PlayerGroundedState
    {
        private Transform _cameraTransform;
        private Vector3 _lastInput;
        private const float EPSILON = 0.0001f;
        private const float LOG_NEGLIGIBLE_RESIDUAL = -4.605170186f; // ≈ ln(0.01), used for precision damping

        public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
            _cameraTransform = Camera.main.transform;
        }

        #region IState Methods
        public override void UpdateState()
        {
            base.UpdateState();

            Move();
            ApplyMotion();
            Rotate();
        }
        #endregion

        #region Main Methods
        // Processes input and calculates desired velocity based on camera direction and movement modifier.
        // Smooths the velocity using angular difference and damping to avoid abrupt changes.
        private void Move()
        {
            // Get the forward direction of the camera (y component ignored for flat movement)
            Vector3 cameraForward = _cameraTransform.forward;
            cameraForward.y = 0;

            // Calculate world-space movement direction relative to camera
            _lastInput = Quaternion.LookRotation(cameraForward.normalized, Vector3.up) * GetMovementInputDirection();

            // Normalize if too large (e.g., diagonal input)
            if (_lastInput.sqrMagnitude > 1)
                _lastInput.Normalize();

            // Calculate desired movement velocity
            Vector3 desiredVelocity = _groundedData.MoveData.BaseSpeed *
                                      _stateMachine.ReusableData.MovementSpeedModifier *
                                      _lastInput;

            // If direction change is small, smoothly rotate using slerp
            if (Vector3.Angle(_stateMachine.ReusableData.CurrentHorizontalVelocity, desiredVelocity) < 100)
            {
                _stateMachine.ReusableData.CurrentHorizontalVelocity = Vector3.Slerp(
                    _stateMachine.ReusableData.CurrentHorizontalVelocity,
                    desiredVelocity,
                    Damp(1, _groundedData.MoveData.BaseRotationData.RotationDamping, Time.deltaTime)
                );
            }
            else
            {
                // Large direction change – add damped velocity difference directly
                _stateMachine.ReusableData.CurrentHorizontalVelocity += Damp(
                    desiredVelocity - _stateMachine.ReusableData.CurrentHorizontalVelocity,
                    _groundedData.MoveData.BaseRotationData.RotationDamping,
                    Time.deltaTime
                );
            }
        }

        // Rotates the player character smoothly toward the movement direction
        private void Rotate()
        {
            // Only rotate if the player has meaningful horizontal velocity
            if (_stateMachine.ReusableData.CurrentHorizontalVelocity.sqrMagnitude > 0.001f)
            {
                // Get the player's current rotation
                Quaternion currentRotation = _stateMachine.PlayerController.transform.rotation;

                // Calculate the rotation the player should face based on velocity
                Quaternion desiredRotation = Quaternion.LookRotation(
                    _stateMachine.ReusableData.CurrentHorizontalVelocity, // Forward direction based on movement
                    Vector3.up // Use world up as the up vector
                );

                // Smoothly interpolate between current and desired rotation using Slerp and damping
                _stateMachine.PlayerController.transform.rotation = Quaternion.Slerp(
                    currentRotation, 
                    desiredRotation, 
                    Damp(1, _groundedData.MoveData.BaseRotationData.RotationDamping, Time.deltaTime) // Smoothed step value
                );
            }
        }

        // Converts 2D movement input into a 3D direction vector on the XZ-plane
        protected Vector3 GetMovementInputDirection() =>
            new(_stateMachine.ReusableData.MovementInput.x, 0f, _stateMachine.ReusableData.MovementInput.y);

        // Applies exponential damping to a float value.
        // Smooths a transition over time toward zero or a target.
        private static float Damp(float initial, float dampTime, float deltaTime)
        {
            // If damping time or input is too small, return input as-is (no damping needed)
            if (dampTime < EPSILON || Mathf.Abs(initial) < EPSILON)
                return initial;

            // If delta time is too small (e.g. paused or single frame), return 0 damping
            if (deltaTime < EPSILON)
                return 0;

            // Calculate damped value using exponential decay (from Unity mathematics)
            return initial * (1 - Mathf.Exp(LOG_NEGLIGIBLE_RESIDUAL * deltaTime / dampTime));
        }

        // Applies damping to each component of a Vector3 using the scalar Damp method
        private Vector3 Damp(Vector3 initial, float dampTime, float deltaTime)
        {
            // Loop through each component (x, y, z)
            for (int i = 0; i < 3; ++i)
                initial[i] = Damp(initial[i], dampTime, deltaTime); // Apply scalar damp to each axis

            return initial; // Return the smoothed vector
        }
        #endregion
    }
}