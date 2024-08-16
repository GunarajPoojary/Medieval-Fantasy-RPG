using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Player.StateMachine
{
    public class PlayerJumpingState : PlayerAirborneState
    {
        private bool _shouldKeepRotating;
        private bool _canStartFalling;

        public PlayerJumpingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            _stateMachine.ReusableData.MovementSpeedModifier = 0f;

            _stateMachine.ReusableData.MovementDecelerationForce = _airborneData.JumpData.DecelerationForce;

            _stateMachine.ReusableData.RotationData = _airborneData.JumpData.RotationData;

            _shouldKeepRotating = _stateMachine.ReusableData.MovementInput != Vector2.zero;

            Jump();
        }

        public override void Exit()
        {
            base.Exit();

            SetBaseRotationData();

            _canStartFalling = false;
        }

        public override void Update()
        {
            base.Update();

            if (!_canStartFalling && IsMovingUp(0f))
            {
                _canStartFalling = true;
            }

            if (!_canStartFalling || IsMovingUp(0f))
            {
                return;
            }

            _stateMachine.ChangeState(_stateMachine.FallingState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (_shouldKeepRotating)
            {
                RotateTowardsTargetRotation();
            }

            if (IsMovingUp())
            {
                DecelerateVertically();
            }
        }
        #endregion

        #region Main Methods
        private void Jump()
        {
            Vector3 jumpForce = _stateMachine.ReusableData.CurrentJumpForce;

            Vector3 jumpDirection = _stateMachine.Player.transform.forward;

            if (_shouldKeepRotating)
            {
                UpdateTargetRotation(GetMovementInputDirection());

                jumpDirection = GetTargetRotationDirection(_stateMachine.ReusableData.CurrentTargetRotation.y);
            }

            jumpForce.x *= jumpDirection.x;
            jumpForce.z *= jumpDirection.z;

            jumpForce = GetJumpForceOnSlope(jumpForce);

            ResetVelocity();

            _stateMachine.Player.Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
        }

        private Vector3 GetJumpForceOnSlope(Vector3 jumpForce)
        {
            Vector3 capsuleColliderCenterInWorldSpace = _stateMachine.Player.ResizableCapsuleCollider.CapsuleColliderData.Collider.bounds.center;

            Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

            if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, _airborneData.JumpData.JumpToGroundRayDistance, _stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

                if (IsMovingUp())
                {
                    float forceModifier = _airborneData.JumpData.JumpForceModifierOnSlopeUpwards.Evaluate(groundAngle);

                    jumpForce.x *= forceModifier;
                    jumpForce.z *= forceModifier;
                }

                if (IsMovingDown())
                {
                    float forceModifier = _airborneData.JumpData.JumpForceModifierOnSlopeDownwards.Evaluate(groundAngle);

                    jumpForce.y *= forceModifier;
                }
            }

            return jumpForce;
        }
        #endregion

        #region Reusable Method
        protected override void ResetSprintState()
        {
        }
        #endregion

        #region Input Method
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
        }
        #endregion
    }
}