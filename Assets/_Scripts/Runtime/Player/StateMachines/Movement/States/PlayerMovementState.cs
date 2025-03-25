using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG
{
    public class PlayerMovementState : IState
    {
        protected PlayerStateFactory StateFactory;

        protected readonly PlayerGroundedData _groundedData;
        protected readonly PlayerAirborneData _airborneData;

        public PlayerMovementState(PlayerStateFactory playerStateFactory)
        {
            StateFactory = playerStateFactory;

            _groundedData = StateFactory.PlayerController.Data.GroundedData;
            _airborneData = StateFactory.PlayerController.Data.AirborneData;

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
            if (StateFactory.PlayerController.LayerData.IsGroundLayer(collider.gameObject.layer))
            {
                OnContactWithGround(collider);

                return;
            }
        }

        public virtual void OnTriggerExit(Collider collider)
        {
            if (StateFactory.PlayerController.LayerData.IsGroundLayer(collider.gameObject.layer))
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
        private void ReadMovementInput() => StateFactory.ReusableData.MovementInput = StateFactory.PlayerController.Input.PlayerActions.Move.ReadValue<Vector2>();

        private void Move()
        {
            bool shouldConsiderSlopes = true;

            if (StateFactory.ReusableData.MovementInput == Vector2.zero || StateFactory.ReusableData.MovementSpeedModifier == 0f)
            {
                return;
            }

            Vector3 movementDirection = GetMovementInputDirection();

            float targetRotationYAngle = UpdateTargetRotation(movementDirection);

            RotateTowardsTargetRotation();

            Vector3 targetRotationDirection = GetTargetRotationDirection(targetRotationYAngle);

            float movementSpeed = _groundedData.BaseSpeed * StateFactory.ReusableData.MovementSpeedModifier;

            if (shouldConsiderSlopes)
            {
                movementSpeed *= StateFactory.ReusableData.MovementOnSlopesSpeedModifier;
            }

            Vector3 currentPlayerHorizontalVelocity = GetHorizontalVelocity();

            StateFactory.PlayerController.Rigidbody.AddForce(targetRotationDirection * movementSpeed - currentPlayerHorizontalVelocity, ForceMode.VelocityChange);
        }

        private void UpdateMovementAnimation()
        {
            var targetSpeed = UpdateMovementParameter();

            StateFactory.PlayerController.AnimationData.AnimationBlend = Mathf.Lerp(StateFactory.PlayerController.AnimationData.AnimationBlend, targetSpeed, Time.deltaTime * 10.0f);

            if (StateFactory.PlayerController.AnimationData.AnimationBlend < 0.01f)
            {
                StateFactory.PlayerController.AnimationData.AnimationBlend = 0.0f;
            }

            StateFactory.PlayerController.Animator.SetFloat(StateFactory.PlayerController.AnimationData.SpeedParameterHash, StateFactory.PlayerController.AnimationData.AnimationBlend);
        }

        private float UpdateMovementParameter()
        {
            float targetSpeed;

            if (StateFactory.ReusableData.MovementInput != Vector2.zero)
            {
                if (StateFactory.ReusableData.ShouldRun)
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
            StateFactory.ReusableData.RotationData = _groundedData.BaseRotationData;

            StateFactory.ReusableData.TimeToReachTargetRotation = StateFactory.ReusableData.RotationData.TargetRotationReachTime;
        }

        protected void StartAnimation(int animationHash) => StateFactory.PlayerController.Animator.SetBool(animationHash, true);

        protected void StopAnimation(int animationHash) => StateFactory.PlayerController.Animator.SetBool(animationHash, false);

        protected virtual void AddInputActionsCallbacks()
        {
            StateFactory.PlayerController.Input.PlayerActions.Run.performed += OnRun;
            StateFactory.PlayerController.Input.PlayerActions.Run.canceled += OnRun;

            StateFactory.PlayerController.Input.PlayerActions.Move.performed += OnMovementPerformed;
            StateFactory.PlayerController.Input.PlayerActions.Move.canceled += OnMovementCanceled;
        }

        protected virtual void RemoveInputActionsCallbacks()
        {
            StateFactory.PlayerController.Input.PlayerActions.Run.performed -= OnRun;
            StateFactory.PlayerController.Input.PlayerActions.Run.canceled -= OnRun;

            StateFactory.PlayerController.Input.PlayerActions.Move.performed -= OnMovementPerformed;
            StateFactory.PlayerController.Input.PlayerActions.Move.canceled -= OnMovementCanceled;
        }

        protected Vector3 GetMovementInputDirection() => new(StateFactory.ReusableData.MovementInput.x, 0f, StateFactory.ReusableData.MovementInput.y);

        protected float UpdateTargetRotation(Vector3 direction)
        {
            float directionAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            if (directionAngle < 0f)
            {
                directionAngle += 360f;
            }

            directionAngle += StateFactory.PlayerController.MainCameraTransform.eulerAngles.y;

            if (directionAngle > 360f)
            {
                directionAngle -= 360f;
            }

            if (directionAngle != StateFactory.ReusableData.CurrentTargetRotation.y)
            {
                StateFactory.ReusableData.CurrentTargetRotation.y = directionAngle;

                StateFactory.ReusableData.DampedTargetRotationPassedTime.y = 0f;
            }

            return directionAngle;
        }

        protected Vector3 GetTargetRotationDirection(float targetRotationAngle) => Quaternion.Euler(0f, targetRotationAngle, 0f) * Vector3.forward;

        protected void RotateTowardsTargetRotation()
        {
            float currentYAngle = StateFactory.PlayerController.Rigidbody.rotation.eulerAngles.y;

            if (currentYAngle == StateFactory.ReusableData.CurrentTargetRotation.y)
            {
                return;
            }

            float smoothedYAngle = Mathf.SmoothDampAngle(currentYAngle, StateFactory.ReusableData.CurrentTargetRotation.y, ref StateFactory.ReusableData.DampedTargetRotationCurrentVelocity.y, StateFactory.ReusableData.TimeToReachTargetRotation.y - StateFactory.ReusableData.DampedTargetRotationPassedTime.y);

            StateFactory.ReusableData.DampedTargetRotationPassedTime.y += Time.deltaTime;

            Quaternion targetRotation = Quaternion.Euler(0f, smoothedYAngle, 0f);

            StateFactory.PlayerController.Rigidbody.MoveRotation(targetRotation);
        }

        protected Vector3 GetHorizontalVelocity()
        {
            Vector3 horizontalVelocity = StateFactory.PlayerController.Rigidbody.linearVelocity;

            horizontalVelocity.y = 0f;

            return horizontalVelocity;
        }

        protected Vector3 GetVerticalVelocity() => new(0f, StateFactory.PlayerController.Rigidbody.linearVelocity.y, 0f);

        protected virtual void OnContactWithGround(Collider collider)
        {
        }

        protected virtual void OnContactWithGroundExited(Collider collider)
        {
        }

        protected void ResetVelocity() => StateFactory.PlayerController.Rigidbody.linearVelocity = Vector3.zero;

        protected void ResetVerticalVelocity()
        {
            Vector3 horizontalVelocity = GetHorizontalVelocity();

            StateFactory.PlayerController.Rigidbody.linearVelocity = horizontalVelocity;
        }

        protected void DecelerateVertically()
        {
            Vector3 verticalVelocity = GetVerticalVelocity();

            StateFactory.PlayerController.Rigidbody.AddForce(-verticalVelocity * StateFactory.ReusableData.MovementDecelerationForce, ForceMode.Acceleration);
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
            StateFactory.ReusableData.ShouldRun = ctx.ReadValueAsButton();
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