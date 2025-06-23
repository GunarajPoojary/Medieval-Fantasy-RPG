using System;
using RPG.Player.Data.States.Airborne;
using RPG.Player.Data.States.Grounded;
using RPG.StateMachine;
using UnityEngine;

namespace RPG.Player.StateMachines.Movement.States
{
    /// <summary>
    /// Base class for player movement-related states that implements the IState interface and handles core movement logics.
    /// </summary>
    public class PlayerBaseMovementState : IState
    {
        protected PlayerStateFactory _stateFactory;

        protected readonly PlayerGroundedData _groundedData;
        protected readonly PlayerAirborneData _airborneData;

        private Transform _mainCameraTransform;

        private const float ANIMATIONBLENDSPEED = 10.0f;
        private const float BLENDSNAPTHRESHOLD = 0.01f;
        private const float FULLROTATIONDEGREES = 360f;
        
        public PlayerBaseMovementState(PlayerStateFactory playerStateFactory)
        {
            _stateFactory = playerStateFactory;

            _groundedData = _stateFactory.PlayerController.Data.GroundedData;
            _airborneData = _stateFactory.PlayerController.Data.AirborneData;

            _mainCameraTransform = Camera.main?.transform;

            SetBaseRotationData();
        }

        #region IState Methods
        public virtual void Enter() => AddInputActionsCallbacks();

        public virtual void Exit() => RemoveInputActionsCallbacks();

        public virtual void HandleInput() => ReadMovementInput();

        public virtual void UpdateState()
        {
            _stateFactory.PlayerController.PreUpdate?.Invoke();

            UpdateMovementAnimation();
            
            if (_stateFactory.PlayerController.PostUpdate != null)
            {
                // Get local-space velocity
                var vel = Quaternion.Inverse(_stateFactory.PlayerController.transform.rotation) * GetHorizontalVelocity();
                vel.y = _stateFactory.PlayerController.Rigidbody.linearVelocity.y;
                //PostUpdate(vel, m_IsSprinting ? JumpSpeed / SprintJumpSpeed : 1);
                _stateFactory.PlayerController.PostUpdate(vel, 1);
            }
        }

        public virtual void PhysicsUpdate() => Move();

        public virtual void OnTriggerEnter(Collider collider)
        {
            if (_stateFactory.PlayerController.LayerData.IsGroundLayer(collider.gameObject.layer))
            {
                OnContactWithGround(collider);
                return;
            }
        }

        public virtual void OnTriggerExit(Collider collider)
        {
            if (_stateFactory.PlayerController.LayerData.IsGroundLayer(collider.gameObject.layer))
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
            _stateFactory.ReusableData.MovementInput = _stateFactory.PlayerController.Input.MoveDirection;

        // Handles actual movement logic
        private void Move()
        {
            bool shouldConsiderSlopes = true;

            // If no input or speed is zero, do not move
            if (_stateFactory.ReusableData.MovementInput == Vector2.zero
                || Mathf.Approximately(_stateFactory.ReusableData.MovementSpeedModifier, 0f))
                return;

            // Get normalized movement input direction
            Vector3 movementDirection = GetMovementInputDirection();

            // Update the target rotation angle based on input direction
            float targetRotationYAngle = UpdateTargetRotation(movementDirection);

            RotateTowardsTargetRotation();

            // Calculate movement direction based on rotation
            Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

            float movementSpeed = _groundedData.BaseSpeed * _stateFactory.ReusableData.MovementSpeedModifier;

            if (shouldConsiderSlopes)
            {
                // Apply slope speed modifier if needed
                movementSpeed *= _stateFactory.ReusableData.MovementOnSlopesSpeedModifier;
            }

            Vector3 currentPlayerHorizontalVelocity = GetHorizontalVelocity();

            // Apply force to move player
            _stateFactory.PlayerController.Rigidbody.AddForce(
                targetRotationDirection * movementSpeed - currentPlayerHorizontalVelocity,
                ForceMode.VelocityChange
            );
        }

        private void UpdateMovementAnimation()
        {
            // Determine target animation speed based on input
            var targetSpeed = UpdateMovementParameter();

            // Smooth blend current animation speed towards target speed
            _stateFactory.PlayerController.AnimationData.AnimationBlend =
                Mathf.Lerp(_stateFactory.PlayerController.AnimationData.AnimationBlend, targetSpeed, Time.deltaTime * ANIMATIONBLENDSPEED);

            // Snap to zero if blend is too low
            if (_stateFactory.PlayerController.AnimationData.AnimationBlend < BLENDSNAPTHRESHOLD)
            {
                _stateFactory.PlayerController.AnimationData.AnimationBlend = 0.0f;
            }

            // Apply animation blend value to Animator
            _stateFactory.PlayerController.Animator.SetFloat(
                _stateFactory.PlayerController.AnimationData.SpeedParameterHash,
                _stateFactory.PlayerController.AnimationData.AnimationBlend
            );
        }

        // Returns speed modifier based on input (idle, walk, run)
        private float UpdateMovementParameter()
        {
            float targetSpeed;

            if (_stateFactory.ReusableData.MovementInput != Vector2.zero)
            {
                targetSpeed = _stateFactory.ReusableData.ShouldRun
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
            _stateFactory.ReusableData.RotationData = _groundedData.BaseRotationData;
            _stateFactory.ReusableData.TimeToReachTargetRotation = _stateFactory.ReusableData.RotationData.TargetRotationReachTime;
        }

        protected void StartAnimation(int animationHash) => _stateFactory.PlayerController.Animator.SetBool(animationHash, true);

        protected void StopAnimation(int animationHash) => _stateFactory.PlayerController.Animator.SetBool(animationHash, false);

        protected virtual void AddInputActionsCallbacks()
        {
            _stateFactory.PlayerController.Input.RunStartedAction += OnRun;
            _stateFactory.PlayerController.Input.RunCanceledAction += OnRun;

            _stateFactory.PlayerController.Input.MovePerformedAction += OnMovementPerformed;
            _stateFactory.PlayerController.Input.MoveCanceledAction += OnMovementCanceled;
        }

        protected virtual void RemoveInputActionsCallbacks()
        {
            _stateFactory.PlayerController.Input.RunStartedAction -= OnRun;
            _stateFactory.PlayerController.Input.RunCanceledAction -= OnRun;

            _stateFactory.PlayerController.Input.MovePerformedAction -= OnMovementPerformed;
            _stateFactory.PlayerController.Input.MoveCanceledAction -= OnMovementCanceled;
        }

        // Converts 2D input into a 3D direction vector
        protected Vector3 GetMovementInputDirection() =>
            new(_stateFactory.ReusableData.MovementInput.x, 0f, _stateFactory.ReusableData.MovementInput.y);

        // Calculates rotation angle based on input and camera direction
        protected float UpdateTargetRotation(Vector3 direction)
        {
            float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (directionAngle < 0f) directionAngle += FULLROTATIONDEGREES;

            // Add camera Y rotation
            directionAngle += _mainCameraTransform.eulerAngles.y;

            if (directionAngle > FULLROTATIONDEGREES) directionAngle -= FULLROTATIONDEGREES;

            // If changed, update target rotation
            if (directionAngle != _stateFactory.ReusableData.CurrentTargetRotation.y)
            {
                _stateFactory.ReusableData.CurrentTargetRotation.y = directionAngle;
                _stateFactory.ReusableData.DampedTargetRotationPassedTime.y = 0f;
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
            float currentYAngle = _stateFactory.PlayerController.Rigidbody.rotation.eulerAngles.y;

            // If the current rotation matches the target, no need to rotate
            if (currentYAngle == _stateFactory.ReusableData.CurrentTargetRotation.y) return;

            // Smoothly interpolate the current angle towards the target angle using damping
            float smoothedYAngle = Mathf.SmoothDampAngle(
                currentYAngle, // current rotation angle
                _stateFactory.ReusableData.CurrentTargetRotation.y, // target rotation angle
                ref _stateFactory.ReusableData.DampedTargetRotationCurrentVelocity.y, // reference to current velocity for damping calculation
                _stateFactory.ReusableData.TimeToReachTargetRotation.y - _stateFactory.ReusableData.DampedTargetRotationPassedTime.y // remaining time to complete rotation
            );

            // Accumulate passed time used for smoothing to calculate remaining smoothing duration
            _stateFactory.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

            // Create a Quaternion with the newly smoothed Y angle, keeping X and Z at 0
            Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);

            // Apply the new rotation to the Rigidbody to rotate the player
            _stateFactory.PlayerController.Rigidbody.MoveRotation(targetRotation);
        }

        protected Vector3 GetHorizontalVelocity()
        {
            Vector3 horizontalVelocity = _stateFactory.PlayerController.Rigidbody.linearVelocity;
            horizontalVelocity.y = 0f;
            return horizontalVelocity;
        }

        protected Vector3 GetVerticalVelocity() =>
            new(0f, _stateFactory.PlayerController.Rigidbody.linearVelocity.y, 0f);

        protected virtual void OnContactWithGround(Collider collider) { }

        protected virtual void OnContactWithGroundExited(Collider collider) { }

        protected void ResetVelocity() =>
            _stateFactory.PlayerController.Rigidbody.linearVelocity = Vector3.zero;

        protected void ResetVerticalVelocity()
        {
            Vector3 horizontalVelocity = GetHorizontalVelocity();
            _stateFactory.PlayerController.Rigidbody.linearVelocity = horizontalVelocity;
        }

        protected void DecelerateVertically()
        {
            Vector3 verticalVelocity = GetVerticalVelocity();
            _stateFactory.PlayerController.Rigidbody.AddForce(-verticalVelocity * _stateFactory.ReusableData.MovementDecelerationForce, ForceMode.Acceleration);
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
        protected virtual void OnRun(bool ShouldRun) => _stateFactory.ReusableData.ShouldRun = ShouldRun;

        protected virtual void OnMovementPerformed(Vector2 moveInput) { }

        protected virtual void OnMovementCanceled(Vector2 moveInput) { }
        #endregion
    }
}