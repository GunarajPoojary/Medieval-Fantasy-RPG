using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG
{
    public class PlayerJumpState : PlayerAirborneState
    {
        private bool _shouldKeepRotating;
        private bool _canStartFalling;

        public PlayerJumpState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            StateFactory.ReusableData.MovementDecelerationForce = _airborneData.JumpData.DecelerationForce;

            StateFactory.ReusableData.RotationData = _airborneData.JumpData.RotationData;

            _shouldKeepRotating = StateFactory.ReusableData.MovementInput != Vector2.zero;

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

            StateFactory.ChangeState(StateFactory.FallState);
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
            Vector3 jumpForce = StateFactory.ReusableData.CurrentJumpForce;

            Vector3 jumpDirection = StateFactory.PlayerController.transform.forward;

            if (_shouldKeepRotating)
            {
                UpdateTargetRotation(GetMovementInputDirection());

                jumpDirection = GetTargetRotationDirection(StateFactory.ReusableData.CurrentTargetRotation.y);
            }

            jumpForce.x *= jumpDirection.x;
            jumpForce.z *= jumpDirection.z;

            jumpForce = GetJumpForceOnSlope(jumpForce);

            ResetVelocity();

            StateFactory.PlayerController.Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
        }

        private Vector3 GetJumpForceOnSlope(Vector3 jumpForce)
        {
            Vector3 capsuleColliderCenterInWorldSpace = StateFactory.PlayerController.ResizableCapsuleCollider.CapsuleColliderData.Collider.bounds.center;

            Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

            if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, _airborneData.JumpData.JumpToGroundRayDistance, StateFactory.PlayerController.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
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

        #region Input Method
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
        }
        #endregion
    }
}