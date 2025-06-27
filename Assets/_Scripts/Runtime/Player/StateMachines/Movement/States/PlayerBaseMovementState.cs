using ProjectEmbersteel.Player.Data.States.Airborne;
using ProjectEmbersteel.Player.Data.States.Grounded;
using ProjectEmbersteel.StateMachine;
using UnityEngine;

namespace ProjectEmbersteel.Player.StateMachines.Movement.States
{
    /// <summary>
    /// Base class for player movement-related states that implements the IState interface and handles core movement logics.
    /// </summary>
    public class PlayerBaseMovementState : IState
    {
        protected PlayerStateFactory _stateMachine;

        protected readonly PlayerGroundedData _groundedData;
        protected readonly PlayerAirborneData _airborneData;

        private const float ANIMATIONBLENDSPEED = 10.0f;
        private const float BLENDSNAPTHRESHOLD = 0.01f;

        public PlayerBaseMovementState(PlayerStateFactory stateMachine)
        {
            _stateMachine = stateMachine;

            _groundedData = _stateMachine.PlayerController.Data.GroundedData;
            _airborneData = _stateMachine.PlayerController.Data.AirborneData;

            SetBaseRotationData();
        }

        #region IState Methods
        public virtual void Enter() => AddInputActionsCallbacks();

        public virtual void Exit() => RemoveInputActionsCallbacks();

        public virtual void HandleInput() => ReadMovementInput();

        public virtual void UpdateState() => UpdateMovementAnimation();

        public virtual void PhysicsUpdate() => Move();

        public virtual void OnTriggerEnter(Collider collider)
        {
            if (_stateMachine.PlayerController.LayerData.IsGroundLayer(collider.gameObject.layer))
            {
                OnContactWithGround(collider);
                return;
            }
        }

        public virtual void OnTriggerExit(Collider collider)
        {
            if (_stateMachine.PlayerController.LayerData.IsGroundLayer(collider.gameObject.layer))
            {
                OnContactWithGroundExited(collider);
                return;
            }
        }

        public virtual void OnAnimationEnterEvent() { }
        public virtual void OnAnimationExitEvent() { }
        public virtual void OnAnimationTransitionEvent() { }
        #endregion

        #region Main Methods
        // Reads movement input from Input System
        private void ReadMovementInput() =>
            _stateMachine.ReusableData.MovementInput = _stateMachine.PlayerController.Input.MoveDirection;

        // Handles actual movement logic
        private void Move()
        {
            bool shouldConsiderSlopes = true;

            // If no input or speed is zero, do not move
            if (_stateMachine.ReusableData.MovementInput == Vector2.zero
                || Mathf.Approximately(_stateMachine.ReusableData.MovementSpeedModifier, 0f))
                return;

            // Get normalized movement input direction
            Vector3 movementDirection = GetMovementInputDirection();

            // Update the target rotation angle based on input direction
            float targetRotationYAngle = UpdateTargetRotation(movementDirection);

            RotateTowardsTargetRotation();

            // Calculate movement direction based on rotation
            Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

            float movementSpeed = _groundedData.BaseSpeed * _stateMachine.ReusableData.MovementSpeedModifier;

            if (shouldConsiderSlopes)
            {
                // Apply slope speed modifier if needed
                movementSpeed *= _stateMachine.ReusableData.MovementOnSlopesSpeedModifier;
            }

            Vector3 currentPlayerHorizontalVelocity = GetHorizontalVelocity();

            // Apply force to move player
            _stateMachine.PlayerController.Rigidbody.AddForce(
                targetRotationDirection * movementSpeed - currentPlayerHorizontalVelocity,
                ForceMode.VelocityChange
            );
        }

        private void UpdateMovementAnimation()
        {
            // Determine target animation speed based on input
            float targetSpeed = UpdateMovementParameter();

            // Smooth blend current animation speed towards target speed
            _stateMachine.PlayerController.AnimationData.AnimationBlend =
                Mathf.Lerp(_stateMachine.PlayerController.AnimationData.AnimationBlend, targetSpeed, Time.deltaTime * ANIMATIONBLENDSPEED);

            // Snap to zero if blend is too low
            if (_stateMachine.PlayerController.AnimationData.AnimationBlend < BLENDSNAPTHRESHOLD)

                _stateMachine.PlayerController.AnimationData.AnimationBlend = 0.0f;

            // Apply animation blend value to Animator
            _stateMachine.PlayerController.Animator.SetFloat(
                _stateMachine.PlayerController.AnimationData.SpeedParameterHash,
                _stateMachine.PlayerController.AnimationData.AnimationBlend
            );
        }

        // Returns speed modifier based on input (idle, walk, run)
        private float UpdateMovementParameter()
        {
            float targetSpeed;

            if (_stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                targetSpeed = _stateMachine.ReusableData.ShouldRun
                    ? _groundedData.RunData.SpeedModifier
                    : _groundedData.WalkData.SpeedModifier;
            }
            else
            {
                targetSpeed = _groundedData.IdleData.SpeedModifier;
            }

            return targetSpeed;
        }
        #endregion

        #region Reusable Methods
        // Sets base rotation configuration from grounded data
        protected void SetBaseRotationData()
        {
            _stateMachine.ReusableData.RotationData = _groundedData.BaseRotationData;
            _stateMachine.ReusableData.TimeToReachTargetRotation = _stateMachine.ReusableData.RotationData.TargetRotationReachTime;
        }

        protected void StartAnimation(int animationHash) => _stateMachine.PlayerController.Animator.SetBool(animationHash, true);

        protected void StopAnimation(int animationHash) => _stateMachine.PlayerController.Animator.SetBool(animationHash, false);

        protected virtual void AddInputActionsCallbacks()
        {
            _stateMachine.PlayerController.Input.RunStartedAction += OnRun;
            _stateMachine.PlayerController.Input.RunCanceledAction += OnRun;

            _stateMachine.PlayerController.Input.MovePerformedAction += OnMovementPerformed;
            _stateMachine.PlayerController.Input.MoveCanceledAction += OnMovementCanceled;
        }

        protected virtual void RemoveInputActionsCallbacks()
        {
            _stateMachine.PlayerController.Input.RunStartedAction -= OnRun;
            _stateMachine.PlayerController.Input.RunCanceledAction -= OnRun;

            _stateMachine.PlayerController.Input.MovePerformedAction -= OnMovementPerformed;
            _stateMachine.PlayerController.Input.MoveCanceledAction -= OnMovementCanceled;
        }

        // Converts 2D input into a 3D direction vector
        protected Vector3 GetMovementInputDirection() =>
            new(_stateMachine.ReusableData.MovementInput.x, 0f, _stateMachine.ReusableData.MovementInput.y);

        // Calculates rotation angle based on input and camera direction
        protected float UpdateTargetRotation(Vector3 direction)
        {
            float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (directionAngle < 0f) directionAngle += 360f;

            // Add camera Y rotation
            directionAngle += Camera.main.transform.eulerAngles.y;

            if (directionAngle > 360f) directionAngle -= 360f;

            // If changed, update target rotation
            if (directionAngle != _stateMachine.ReusableData.CurrentTargetRotation.y)
            {
                _stateMachine.ReusableData.CurrentTargetRotation.y = directionAngle;
                _stateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0f;
            }

            return directionAngle;
        }

        // Converts rotation angle to a forward vector
        protected Vector3 GetTargetRotationDirection(float targetRotationAngle) =>
            Quaternion.Euler(0f, targetRotationAngle, 0f) * Vector3.forward;

        // Smoothly rotates player toward target rotation
        protected void RotateTowardsTargetRotation()
        {
            // Get the current Y-axis rotation of the player's Rigidbody
            float currentYAngle = _stateMachine.PlayerController.Rigidbody.rotation.eulerAngles.y;

            // If the current rotation matches the target, no need to rotate
            if (currentYAngle == _stateMachine.ReusableData.CurrentTargetRotation.y) return;

            // Smoothly interpolate the current angle towards the target angle using damping
            float smoothedYAngle = Mathf.SmoothDampAngle(
                currentYAngle, // current rotation angle
                _stateMachine.ReusableData.CurrentTargetRotation.y, // target rotation angle
                ref _stateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y, // reference to current velocity for damping calculation
                _stateMachine.ReusableData.TimeToReachTargetRotation.y - _stateMachine.ReusableData.DampedTargetRotationPassedTime.y // remaining time to complete rotation
            );

            // Accumulate passed time used for smoothing to calculate remaining smoothing duration
            _stateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

            // Create a Quaternion with the newly smoothed Y angle, keeping X and Z at 0
            Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);

            // Apply the new rotation to the Rigidbody to rotate the player
            _stateMachine.PlayerController.Rigidbody.MoveRotation(targetRotation);
        }

        protected Vector3 GetHorizontalVelocity()
        {
            Vector3 horizontalVelocity = _stateMachine.PlayerController.Rigidbody.linearVelocity;
            horizontalVelocity.y = 0f;
            return horizontalVelocity;
        }

        protected Vector3 GetVerticalVelocity() =>
            new(0f, _stateMachine.PlayerController.Rigidbody.linearVelocity.y, 0f);

        protected virtual void OnContactWithGround(Collider collider) { }

        protected virtual void OnContactWithGroundExited(Collider collider) { }

        protected void ResetVelocity() =>
            _stateMachine.PlayerController.Rigidbody.linearVelocity = Vector3.zero;

        protected void ResetVerticalVelocity()
        {
            Vector3 horizontalVelocity = GetHorizontalVelocity();
            _stateMachine.PlayerController.Rigidbody.linearVelocity = horizontalVelocity;
        }

        protected void DecelerateVertically()
        {
            Vector3 verticalVelocity = GetVerticalVelocity();
            _stateMachine.PlayerController.Rigidbody.AddForce(-verticalVelocity * _stateMachine.ReusableData.MovementDecelerationForce, ForceMode.Acceleration);
        }

        // Checks if horizontal movement exceeds a threshold
        protected bool IsMovingHorizontally(float minimumMagnitude = 0.1f)
        {
            Vector3 horizontalVelocity = GetHorizontalVelocity();
            Vector2 horizontalMovement = new Vector2(horizontalVelocity.x, horizontalVelocity.z);
            return horizontalMovement.magnitude > minimumMagnitude;
        }

        // Checks if vertical movement is upward
        protected bool IsMovingUp(float minimumVelocity = 0.1f) =>
            GetVerticalVelocity().y > minimumVelocity;

        // Checks if vertical movement is downward
        protected bool IsMovingDown(float minimumVelocity = 0.1f) =>
            GetVerticalVelocity().y < -minimumVelocity;
        #endregion

        #region Input Methods
        protected virtual void OnRun(bool ShouldRun) => _stateMachine.ReusableData.ShouldRun = ShouldRun;

        protected virtual void OnMovementPerformed(Vector2 moveInput) { }

        protected virtual void OnMovementCanceled(Vector2 moveInput) { }
        #endregion
    }
}