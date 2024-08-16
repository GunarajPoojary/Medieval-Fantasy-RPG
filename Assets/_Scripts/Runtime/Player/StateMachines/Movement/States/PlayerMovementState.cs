using RPG.Player.Data;
using RPG.Player.Data.States;
using RPG.StateMachine;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Player.StateMachine
{
    public class PlayerMovementState : IState
    {
        protected PlayerMovementStateMachine _stateMachine;

        protected readonly PlayerGroundedData _groundedData;
        protected readonly PlayerAirborneData _airborneData;

        public PlayerMovementState(PlayerMovementStateMachine playerMovementStateMachine)
        {
            _stateMachine = playerMovementStateMachine;

            _groundedData = _stateMachine.Player.Data.GroundedData;
            _airborneData = _stateMachine.Player.Data.AirborneData;

            InitializeData();
        }

        private void InitializeData()
        {
            SetBaseCameraRecenteringData();

            SetBaseRotationData();
        }

        #region IState Methods
        public virtual void Enter() => AddInputActionsCallbacks();

        public virtual void Exit() => RemoveInputActionsCallbacks();

        public virtual void HandleInput() => ReadMovementInput();

        public virtual void Update()
        {
        }

        public virtual void PhysicsUpdate() => Move();

        public virtual void OnTriggerEnter(Collider collider)
        {
            if (_stateMachine.Player.LayerData.IsGroundLayer(collider.gameObject.layer))
            {
                OnContactWithGround(collider);

                return;
            }
        }

        public virtual void OnTriggerExit(Collider collider)
        {
            if (_stateMachine.Player.LayerData.IsGroundLayer(collider.gameObject.layer))
            {
                OnContactWithGroundExited(collider);

                return;
            }
        }

        public virtual void OnAnimationEnterEvent()
        {
        }

        public virtual void OnAnimationExitEvent()
        {
        }

        public virtual void OnAnimationTransitionEvent()
        {
        }
        #endregion

        #region Main Methods
        private void ReadMovementInput() => _stateMachine.ReusableData.MovementInput = _stateMachine.Player.Input.PlayerActions.Movement.ReadValue<Vector2>();

        private void Move()
        {
            if (_stateMachine.ReusableData.MovementInput == Vector2.zero || _stateMachine.ReusableData.MovementSpeedModifier == 0f)
            {
                return;
            }

            Vector3 movementDirection = GetMovementInputDirection();

            float targetRotationYAngle = Rotate(movementDirection);

            Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

            float movementSpeed = GetMovementSpeed();

            Vector3 currentPlayerHorizontalVelocity = GetPlayerHorizontalVelocity();

            _stateMachine.Player.Rigidbody.AddForce(targetRotationDirection * movementSpeed - currentPlayerHorizontalVelocity, ForceMode.VelocityChange);
        }

        private float Rotate(Vector3 direction)
        {
            float directionAngle = UpdateTargetRotation(direction);

            RotateTowardsTargetRotation();

            return directionAngle;
        }

        private float GetDirectionAngle(Vector3 direction)
        {
            float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (directionAngle < 0f)
            {
                directionAngle += 360f;
            }

            return directionAngle;
        }

        private float AddCameraRotationToAngle(float angle)
        {
            angle += _stateMachine.Player.MainCameraTransform.eulerAngles.y;

            if (angle > 360f)
            {
                angle -= 360f;
            }

            return angle;
        }

        private void UpdateTargetRotationData(float targetAngle)
        {
            _stateMachine.ReusableData.CurrentTargetRotation.y = targetAngle;

            _stateMachine.ReusableData.DampedTargetRotationPassedTime.y = 0f;
        }
        #endregion

        #region Reusable Methods
        protected void SetBaseCameraRecenteringData()
        {
            _stateMachine.ReusableData.SidewaysCameraRecenteringData = _groundedData.SidewaysCameraRecenteringData;
            _stateMachine.ReusableData.BackwardsCameraRecenteringData = _groundedData.BackwardsCameraRecenteringData;
        }

        protected void SetBaseRotationData()
        {
            _stateMachine.ReusableData.RotationData = _groundedData.BaseRotationData;

            _stateMachine.ReusableData.TimeToReachTargetRotation = _stateMachine.ReusableData.RotationData.TargetRotationReachTime;
        }

        protected void StartAnimation(int animationHash) => _stateMachine.Player.Animator.SetBool(animationHash, true);

        protected void StopAnimation(int animationHash) => _stateMachine.Player.Animator.SetBool(animationHash, false);

        protected virtual void AddInputActionsCallbacks()
        {
            _stateMachine.Player.Input.PlayerActions.WalkToggle.started += OnWalkToggleStarted;

            _stateMachine.Player.Input.PlayerActions.Look.started += OnMouseMovementStarted;

            _stateMachine.Player.Input.PlayerActions.Movement.performed += OnMovementPerformed;
            _stateMachine.Player.Input.PlayerActions.Movement.canceled += OnMovementCanceled;
        }

        protected virtual void RemoveInputActionsCallbacks()
        {
            _stateMachine.Player.Input.PlayerActions.WalkToggle.started -= OnWalkToggleStarted;

            _stateMachine.Player.Input.PlayerActions.Look.started -= OnMouseMovementStarted;

            _stateMachine.Player.Input.PlayerActions.Movement.performed -= OnMovementPerformed;
            _stateMachine.Player.Input.PlayerActions.Movement.canceled -= OnMovementCanceled;
        }

        protected Vector3 GetMovementInputDirection() => new Vector3(_stateMachine.ReusableData.MovementInput.x, 0f, _stateMachine.ReusableData.MovementInput.y);

        protected float UpdateTargetRotation(Vector3 direction, bool shouldConsiderCameraRotation = true)
        {
            float directionAngle = GetDirectionAngle(direction);

            if (shouldConsiderCameraRotation)
            {
                directionAngle = AddCameraRotationToAngle(directionAngle);
            }

            if (directionAngle != _stateMachine.ReusableData.CurrentTargetRotation.y)
            {
                UpdateTargetRotationData(directionAngle);
            }

            return directionAngle;
        }

        protected void RotateTowardsTargetRotation()
        {
            float currentYAngle = _stateMachine.Player.Rigidbody.rotation.eulerAngles.y;

            if (currentYAngle == _stateMachine.ReusableData.CurrentTargetRotation.y)
            {
                return;
            }

            float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, _stateMachine.ReusableData.CurrentTargetRotation.y, ref _stateMachine.ReusableData.DampedTargetRotationCurrentVelocity.y, _stateMachine.ReusableData.TimeToReachTargetRotation.y - _stateMachine.ReusableData.DampedTargetRotationPassedTime.y);

            _stateMachine.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

            Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);

            _stateMachine.Player.Rigidbody.MoveRotation(targetRotation);
        }

        protected Vector3 GetTargetRotationDirection(float targetRotationAngle) => Quaternion.Euler(0f, targetRotationAngle, 0f) * Vector3.forward;

        protected float GetMovementSpeed(bool shouldConsiderSlopes = true)
        {
            float movementSpeed = _groundedData.BaseSpeed * _stateMachine.ReusableData.MovementSpeedModifier;

            if (shouldConsiderSlopes)
            {
                movementSpeed *= _stateMachine.ReusableData.MovementOnSlopesSpeedModifier;
            }

            return movementSpeed;
        }

        protected Vector3 GetPlayerHorizontalVelocity()
        {
            Vector3 playerHorizontalVelocity = _stateMachine.Player.Rigidbody.velocity;

            playerHorizontalVelocity.y = 0f;

            return playerHorizontalVelocity;
        }

        protected Vector3 GetPlayerVerticalVelocity() => new Vector3(0f, _stateMachine.Player.Rigidbody.velocity.y, 0f);

        protected virtual void OnContactWithGround(Collider collider)
        {
        }

        protected virtual void OnContactWithGroundExited(Collider collider)
        {
        }

        protected void UpdateCameraRecenteringState(Vector2 movementInput)
        {
            if (movementInput == Vector2.zero)
            {
                return;
            }

            if (movementInput == Vector2.up)
            {
                DisableCameraRecentering();

                return;
            }

            float cameraVerticalAngle = _stateMachine.Player.MainCameraTransform.eulerAngles.x;

            if (cameraVerticalAngle >= 270f)
            {
                cameraVerticalAngle -= 360f;
            }

            cameraVerticalAngle = Mathf.Abs(cameraVerticalAngle);

            if (movementInput == Vector2.down)
            {
                SetCameraRecenteringState(cameraVerticalAngle, _stateMachine.ReusableData.BackwardsCameraRecenteringData);

                return;
            }

            SetCameraRecenteringState(cameraVerticalAngle, _stateMachine.ReusableData.SidewaysCameraRecenteringData);
        }

        protected void SetCameraRecenteringState(float cameraVerticalAngle, List<PlayerCameraRecenteringData> cameraRecenteringData)
        {
            foreach (PlayerCameraRecenteringData recenteringData in cameraRecenteringData)
            {
                if (!recenteringData.IsWithinRange(cameraVerticalAngle))
                {
                    continue;
                }

                EnableCameraRecentering(recenteringData.WaitTime, recenteringData.RecenteringTime);

                return;
            }

            DisableCameraRecentering();
        }

        protected void EnableCameraRecentering(float waitTime = -1f, float recenteringTime = -1f)
        {
            float movementSpeed = GetMovementSpeed();

            if (movementSpeed == 0f)
            {
                movementSpeed = _groundedData.BaseSpeed;
            }

            _stateMachine.Player.CameraRecenteringUtility.EnableRecentering(waitTime, recenteringTime, _groundedData.BaseSpeed, movementSpeed);
        }

        protected void DisableCameraRecentering() => _stateMachine.Player.CameraRecenteringUtility.DisableRecentering();

        protected void ResetVelocity() => _stateMachine.Player.Rigidbody.velocity = Vector3.zero;

        protected void ResetVerticalVelocity()
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();

            _stateMachine.Player.Rigidbody.velocity = playerHorizontalVelocity;
        }

        protected void DecelerateHorizontally()
        {
            Vector3 playerHorizontalVelocity = GetPlayerHorizontalVelocity();

            _stateMachine.Player.Rigidbody.AddForce(-playerHorizontalVelocity * _stateMachine.ReusableData.MovementDecelerationForce, ForceMode.Acceleration);
        }

        protected void DecelerateVertically()
        {
            Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();

            _stateMachine.Player.Rigidbody.AddForce(-playerVerticalVelocity * _stateMachine.ReusableData.MovementDecelerationForce, ForceMode.Acceleration);
        }

        protected bool IsMovingHorizontally(float minimumMagnitude = 0.1f)
        {
            Vector3 playerHorizontaVelocity = GetPlayerHorizontalVelocity();

            Vector2 playerHorizontalMovement = new Vector2(playerHorizontaVelocity.x, playerHorizontaVelocity.z);

            return playerHorizontalMovement.magnitude > minimumMagnitude;
        }

        protected bool IsMovingUp(float minimumVelocity = 0.1f) => GetPlayerVerticalVelocity().y > minimumVelocity;

        protected bool IsMovingDown(float minimumVelocity = 0.1f) => GetPlayerVerticalVelocity().y < -minimumVelocity;
        #endregion

        #region Input Methods
        protected virtual void OnWalkToggleStarted(InputAction.CallbackContext context) => _stateMachine.ReusableData.ShouldWalk = !_stateMachine.ReusableData.ShouldWalk;

        private void OnMouseMovementStarted(InputAction.CallbackContext context) => UpdateCameraRecenteringState(_stateMachine.ReusableData.MovementInput);

        protected virtual void OnMovementPerformed(InputAction.CallbackContext context) => UpdateCameraRecenteringState(context.ReadValue<Vector2>());

        protected virtual void OnMovementCanceled(InputAction.CallbackContext context) => DisableCameraRecentering();
        #endregion
    }
}