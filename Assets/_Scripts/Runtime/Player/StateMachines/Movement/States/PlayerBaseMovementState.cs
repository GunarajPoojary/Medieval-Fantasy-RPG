using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG
{
    public class PlayerBaseMovementState : IState
    {
        protected PlayerStateFactory _stateFactory;

        protected readonly PlayerGroundedData _groundedData;
        protected readonly PlayerAirborneData _airborneData;

        public PlayerBaseMovementState(PlayerStateFactory playerStateFactory)
        {
            _stateFactory = playerStateFactory;

            _groundedData = _stateFactory.PlayerController.Data.GroundedData;
            _airborneData = _stateFactory.PlayerController.Data.AirborneData;

            SetBaseRotationData();
        }

        #region IState Methods
        public virtual void Enter() => AddInputActionsCallbacks();

        public virtual void Exit() => RemoveInputActionsCallbacks();

        public virtual void HandleInput() => ReadMovementInput();

        public virtual void Update() => UpdateMovementAnimation();

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
        private void ReadMovementInput() => _stateFactory.ReusableData.MovementInput = _stateFactory.PlayerController.Input.PlayerActions.Move.ReadValue<Vector2>();

        private void Move()
        {
            bool shouldConsiderSlopes = true;

            if (_stateFactory.ReusableData.MovementInput == Vector2.zero || _stateFactory.ReusableData.MovementSpeedModifier == 0f)
            {
                return;
            }

            Vector3 movementDirection = GetMovementInputDirection();

            float targetRotationYAngle = UpdateTargetRotation(movementDirection);

            RotateTowardsTargetRotation();

            Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

            float movementSpeed = _groundedData.BaseSpeed * _stateFactory.ReusableData.MovementSpeedModifier;

            if (shouldConsiderSlopes)
            {
                movementSpeed *= _stateFactory.ReusableData.MovementOnSlopesSpeedModifier;
            }

            Vector3 currentPlayerHorizontalVelocity = GetHorizontalVelocity();

            _stateFactory.PlayerController.Rigidbody.AddForce(targetRotationDirection * movementSpeed - currentPlayerHorizontalVelocity, ForceMode.VelocityChange);
        }

        private void UpdateMovementAnimation()
        {
            var targetSpeed = UpdateMovementParameter();

            _stateFactory.PlayerController.AnimationData.AnimationBlend = Mathf.Lerp(_stateFactory.PlayerController.AnimationData.AnimationBlend, targetSpeed, Time.deltaTime * 10.0f);

            if (_stateFactory.PlayerController.AnimationData.AnimationBlend < 0.01f)
            {
                _stateFactory.PlayerController.AnimationData.AnimationBlend = 0.0f;
            }

            _stateFactory.PlayerController.Animator.SetFloat(_stateFactory.PlayerController.AnimationData.SpeedParameterHash, _stateFactory.PlayerController.AnimationData.AnimationBlend);
        }

        private float UpdateMovementParameter()
        {
            float targetSpeed;

            if (_stateFactory.ReusableData.MovementInput != Vector2.zero)
            {
                if (_stateFactory.ReusableData.ShouldRun)
                {
                    targetSpeed = _groundedData.RunData.SpeedModifier;
                }
                else
                {
                    targetSpeed = _groundedData.WalkData.SpeedModifier;
                }
            }
            else
            {
                targetSpeed = _groundedData.IdleData.SpeedModifier;
            }

            return targetSpeed;
        }
        #endregion

        #region Reusable Methods
        protected void SetBaseRotationData()
        {
            _stateFactory.ReusableData.RotationData = _groundedData.BaseRotationData;

            _stateFactory.ReusableData.TimeToReachTargetRotation = _stateFactory.ReusableData.RotationData.TargetRotationReachTime;
        }

        protected void StartAnimation(int animationHash) => _stateFactory.PlayerController.Animator.SetBool(animationHash, true);

        protected void StopAnimation(int animationHash) => _stateFactory.PlayerController.Animator.SetBool(animationHash, false);

        protected virtual void AddInputActionsCallbacks()
        {
            _stateFactory.PlayerController.Input.PlayerActions.Run.performed += OnRun;
            _stateFactory.PlayerController.Input.PlayerActions.Run.canceled += OnRun;

            _stateFactory.PlayerController.Input.PlayerActions.Move.performed += OnMovementPerformed;
            _stateFactory.PlayerController.Input.PlayerActions.Move.canceled += OnMovementCanceled;
        }

        protected virtual void RemoveInputActionsCallbacks()
        {
            _stateFactory.PlayerController.Input.PlayerActions.Run.performed -= OnRun;
            _stateFactory.PlayerController.Input.PlayerActions.Run.canceled -= OnRun;

            _stateFactory.PlayerController.Input.PlayerActions.Move.performed -= OnMovementPerformed;
            _stateFactory.PlayerController.Input.PlayerActions.Move.canceled -= OnMovementCanceled;
        }

        protected Vector3 GetMovementInputDirection() => new(_stateFactory.ReusableData.MovementInput.x, 0f, _stateFactory.ReusableData.MovementInput.y);

        protected float UpdateTargetRotation(Vector3 direction)
        {
            float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (directionAngle < 0f)
            {
                directionAngle += 360f;
            }

            directionAngle += _stateFactory.PlayerController.MainCameraTransform.eulerAngles.y;

            if (directionAngle > 360f)
            {
                directionAngle -= 360f;
            }

            if (directionAngle != _stateFactory.ReusableData.CurrentTargetRotation.y)
            {
                _stateFactory.ReusableData.CurrentTargetRotation.y = directionAngle;

                _stateFactory.ReusableData.DampedTargetRotationPassedTime.y = 0f;
            }

            return directionAngle;
        }

        protected Vector3 GetTargetRotationDirection(float targetRotationAngle) => Quaternion.Euler(0f, targetRotationAngle, 0f) * Vector3.forward;

        protected void RotateTowardsTargetRotation()
        {
            float currentYAngle = _stateFactory.PlayerController.Rigidbody.rotation.eulerAngles.y;

            if (currentYAngle == _stateFactory.ReusableData.CurrentTargetRotation.y)
            {
                return;
            }

            float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, _stateFactory.ReusableData.CurrentTargetRotation.y, ref _stateFactory.ReusableData.DampedTargetRotationCurrentVelocity.y, _stateFactory.ReusableData.TimeToReachTargetRotation.y - _stateFactory.ReusableData.DampedTargetRotationPassedTime.y);

            _stateFactory.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

            Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);

            _stateFactory.PlayerController.Rigidbody.MoveRotation(targetRotation);
        }

        protected Vector3 GetHorizontalVelocity()
        {
            Vector3 horizontalVelocity = _stateFactory.PlayerController.Rigidbody.linearVelocity;

            horizontalVelocity.y = 0f;

            return horizontalVelocity;
        }

        protected Vector3 GetVerticalVelocity() => new(0f, _stateFactory.PlayerController.Rigidbody.linearVelocity.y, 0f);

        protected virtual void OnContactWithGround(Collider collider)
        {
        }

        protected virtual void OnContactWithGroundExited(Collider collider)
        {
        }

        protected void ResetVelocity() => _stateFactory.PlayerController.Rigidbody.linearVelocity = Vector3.zero;

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

        protected bool IsMovingHorizontally(float minimumMagnitude = 0.1f)
        {
            Vector3 horizontalVelocity = GetHorizontalVelocity();

            Vector2 horizontalMovement = new Vector2(horizontalVelocity.x, horizontalVelocity.z);

            return horizontalMovement.magnitude > minimumMagnitude;
        }

        protected bool IsMovingUp(float minimumVelocity = 0.1f) => GetVerticalVelocity().y > minimumVelocity;

        protected bool IsMovingDown(float minimumVelocity = 0.1f) => GetVerticalVelocity().y < -minimumVelocity;
        #endregion

        #region Input Methods
        protected virtual void OnRun(InputAction.CallbackContext ctx)
        {
            _stateFactory.ReusableData.ShouldRun = ctx.ReadValueAsButton();
        }

        protected virtual void OnMovementPerformed(InputAction.CallbackContext ctx)
        {
        }

        protected virtual void OnMovementCanceled(InputAction.CallbackContext ctx)
        {
        }
        #endregion
    }
}