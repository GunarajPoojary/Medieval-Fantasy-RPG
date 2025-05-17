using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG
{
    public class PlayerGroundedState : PlayerBaseMovementState
    {
        public PlayerGroundedState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            StartAnimation(_stateFactory.PlayerController.AnimationData.GroundedParameterHash);

            UpdateShouldRunState();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateFactory.PlayerController.AnimationData.GroundedParameterHash);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            Float();
        }

        private void UpdateShouldRunState()
        {
            if (!_stateFactory.ReusableData.ShouldRun)
            {
                return;
            }

            if (_stateFactory.ReusableData.MovementInput != Vector2.zero)
            {
                return;
            }

            _stateFactory.ReusableData.ShouldRun = false;
        }
        #endregion

        #region Main Methods
        private void Float()
        {
            Vector3 capsuleColliderCenterInWorldSpace = _stateFactory.PlayerController.ResizableCapsuleCollider.CapsuleColliderData.Collider.bounds.center;

            Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

            if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, _stateFactory.PlayerController.ResizableCapsuleCollider.SlopeData.FloatRayDistance, _stateFactory.PlayerController.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

                float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);

                if (slopeSpeedModifier == 0f)
                {
                    return;
                }

                float distanceToFloatingPoint = _stateFactory.PlayerController.ResizableCapsuleCollider.CapsuleColliderData.ColliderCenterInLocalSpace.y * _stateFactory.PlayerController.transform.localScale.y - hit.distance;

                if (distanceToFloatingPoint == 0f)
                {
                    return;
                }

                float amountToLift = distanceToFloatingPoint * _stateFactory.PlayerController.ResizableCapsuleCollider.SlopeData.StepReachForce - GetVerticalVelocity().y;

                Vector3 liftForce = new Vector3(0f, amountToLift, 0f);

                _stateFactory.PlayerController.Rigidbody.AddForce(liftForce, ForceMode.VelocityChange);
            }
        }

        private float SetSlopeSpeedModifierOnAngle(float angle)
        {
            float slopeSpeedModifier = _groundedData.SlopeSpeedAngles.Evaluate(angle);

            if (_stateFactory.ReusableData.MovementOnSlopesSpeedModifier != slopeSpeedModifier)
            {
                _stateFactory.ReusableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;
            }

            return slopeSpeedModifier;
        }

        private bool IsThereGroundUnderneath()
        {
            PlayerTriggerColliderData triggerColliderData = _stateFactory.PlayerController.ResizableCapsuleCollider.TriggerColliderData;

            Vector3 groundColliderCenterInWorldSpace = triggerColliderData.GroundCheckCollider.bounds.center;

            Collider[] overlappedGroundColliders = Physics.OverlapBox(groundColliderCenterInWorldSpace, triggerColliderData.GroundCheckColliderVerticalExtents, triggerColliderData.GroundCheckCollider.transform.rotation, _stateFactory.PlayerController.LayerData.GroundLayer, QueryTriggerInteraction.Ignore);

            return overlappedGroundColliders.Length > 0;
        }
        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            _stateFactory.PlayerController.Input.PlayerActions.Jump.started += OnJumpStarted;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();

            _stateFactory.PlayerController.Input.PlayerActions.Jump.started -= OnJumpStarted;
        }

        protected virtual void OnMove()
        {
            if (_stateFactory.ReusableData.ShouldRun)
            {
                _stateFactory.SwitchState(_stateFactory.RunState);

                return;
            }

            _stateFactory.SwitchState(_stateFactory.WalkState);
        }

        protected override void OnContactWithGroundExited(Collider collider)
        {
            if (IsThereGroundUnderneath())
            {
                return;
            }

            Vector3 capsuleColliderCenterInWorldSpace = _stateFactory.PlayerController.ResizableCapsuleCollider.CapsuleColliderData.Collider.bounds.center;

            Ray downwardsRayFromCapsuleBottom = new Ray(capsuleColliderCenterInWorldSpace - _stateFactory.PlayerController.ResizableCapsuleCollider.CapsuleColliderData.ColliderVerticalExtents, Vector3.down);

            if (!Physics.Raycast(downwardsRayFromCapsuleBottom, out _, _groundedData.GroundToFallRayDistance, _stateFactory.PlayerController.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                OnFall();
            }
        }

        protected virtual void OnFall() => _stateFactory.SwitchState(_stateFactory.FallState);
        #endregion

        #region Input Methods
        protected override void OnRun(InputAction.CallbackContext ctx)
        {
            base.OnRun(ctx);

            if (_stateFactory.ReusableData.MovementInput != Vector2.zero)
            {
                OnMove();
            }
        }

        protected override void OnMovementPerformed(InputAction.CallbackContext ctx)
        {
            base.OnMovementPerformed(ctx);

            UpdateTargetRotation(GetMovementInputDirection());
        }

        protected virtual void OnJumpStarted(InputAction.CallbackContext ctx)
        {
            _stateFactory.SwitchState(_stateFactory.JumpState);
        }
        #endregion
    }
}