using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG
{
    public class PlayerGroundedState : PlayerMovementState
    {
        public PlayerGroundedState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            StartAnimation(StateFactory.PlayerController.AnimationData.GroundedParameterHash);

            UpdateShouldRunState();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(StateFactory.PlayerController.AnimationData.GroundedParameterHash);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            Float();
        }

        private void UpdateShouldRunState()
        {
            if (!StateFactory.ReusableData.ShouldRun)
            {
                return;
            }

            if (StateFactory.ReusableData.MovementInput != Vector2.zero)
            {
                return;
            }

            StateFactory.ReusableData.ShouldRun = false;
        }
        #endregion

        #region Main Methods
        private void Float()
        {
            Vector3 capsuleColliderCenterInWorldSpace = StateFactory.PlayerController.ResizableCapsuleCollider.CapsuleColliderData.Collider.bounds.center;

            Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

            if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, StateFactory.PlayerController.ResizableCapsuleCollider.SlopeData.FloatRayDistance, StateFactory.PlayerController.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

                float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);

                if (slopeSpeedModifier == 0f)
                {
                    return;
                }

                float distanceToFloatingPoint = StateFactory.PlayerController.ResizableCapsuleCollider.CapsuleColliderData.ColliderCenterInLocalSpace.y * StateFactory.PlayerController.transform.localScale.y - hit.distance;

                if (distanceToFloatingPoint == 0f)
                {
                    return;
                }

                float amountToLift = distanceToFloatingPoint * StateFactory.PlayerController.ResizableCapsuleCollider.SlopeData.StepReachForce - GetVerticalVelocity().y;

                Vector3 liftForce = new Vector3(0f, amountToLift, 0f);

                StateFactory.PlayerController.Rigidbody.AddForce(liftForce, ForceMode.VelocityChange);
            }
        }

        private float SetSlopeSpeedModifierOnAngle(float angle)
        {
            float slopeSpeedModifier = _groundedData.SlopeSpeedAngles.Evaluate(angle);

            if (StateFactory.ReusableData.MovementOnSlopesSpeedModifier != slopeSpeedModifier)
            {
                StateFactory.ReusableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;
            }

            return slopeSpeedModifier;
        }

        private bool IsThereGroundUnderneath()
        {
            PlayerTriggerColliderData triggerColliderData = StateFactory.PlayerController.ResizableCapsuleCollider.TriggerColliderData;

            Vector3 groundColliderCenterInWorldSpace = triggerColliderData.GroundCheckCollider.bounds.center;

            Collider[] overlappedGroundColliders = Physics.OverlapBox(groundColliderCenterInWorldSpace, triggerColliderData.GroundCheckColliderVerticalExtents, triggerColliderData.GroundCheckCollider.transform.rotation, StateFactory.PlayerController.LayerData.GroundLayer, QueryTriggerInteraction.Ignore);

            return overlappedGroundColliders.Length > 0;
        }
        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            StateFactory.PlayerController.Input.PlayerActions.Jump.started += OnJumpStarted;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();

            StateFactory.PlayerController.Input.PlayerActions.Jump.started -= OnJumpStarted;
        }

        protected virtual void OnMove()
        {
            if (StateFactory.ReusableData.ShouldRun)
            {
                StateFactory.ChangeState(StateFactory.RunState);

                return;
            }

            StateFactory.ChangeState(StateFactory.WalkState);
        }

        protected override void OnContactWithGroundExited(Collider collider)
        {
            if (IsThereGroundUnderneath())
            {
                return;
            }

            Vector3 capsuleColliderCenterInWorldSpace = StateFactory.PlayerController.ResizableCapsuleCollider.CapsuleColliderData.Collider.bounds.center;

            Ray downwardsRayFromCapsuleBottom = new Ray(capsuleColliderCenterInWorldSpace - StateFactory.PlayerController.ResizableCapsuleCollider.CapsuleColliderData.ColliderVerticalExtents, Vector3.down);

            if (!Physics.Raycast(downwardsRayFromCapsuleBottom, out _, _groundedData.GroundToFallRayDistance, StateFactory.PlayerController.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                OnFall();
            }
        }

        protected virtual void OnFall() => StateFactory.ChangeState(StateFactory.FallState);
        #endregion

        #region Input Methods
        protected override void OnRun(InputAction.CallbackContext ctx)
        {
            base.OnRun(ctx);

            if (StateFactory.ReusableData.MovementInput != Vector2.zero)
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
            StateFactory.ChangeState(StateFactory.JumpState);
        }
        #endregion
    }
}