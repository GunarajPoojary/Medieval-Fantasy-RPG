using RPG.Player.Data;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Player.StateMachine
{
    public class PlayerGroundedState : PlayerMovementState
    {
        public PlayerGroundedState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            StartAnimation(_stateMachine.Player.AnimationData.GroundedParameterHash);

            UpdateShouldSprintState();

            UpdateCameraRecenteringState(_stateMachine.ReusableData.MovementInput);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.Player.AnimationData.GroundedParameterHash);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            Float();
        }

        private void UpdateShouldSprintState()
        {
            if (!_stateMachine.ReusableData.ShouldSprint)
            {
                return;
            }

            if (_stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                return;
            }

            _stateMachine.ReusableData.ShouldSprint = false;
        }
        #endregion

        #region Main Methods
        private void Float()
        {
            Vector3 capsuleColliderCenterInWorldSpace = _stateMachine.Player.ResizableCapsuleCollider.CapsuleColliderData.Collider.bounds.center;

            Ray downwardsRayFromCapsuleCenter = new Ray(capsuleColliderCenterInWorldSpace, Vector3.down);

            if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit, _stateMachine.Player.ResizableCapsuleCollider.SlopeData.FloatRayDistance, _stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

                float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);

                if (slopeSpeedModifier == 0f)
                {
                    return;
                }

                float distanceToFloatingPoint = _stateMachine.Player.ResizableCapsuleCollider.CapsuleColliderData.ColliderCenterInLocalSpace.y * _stateMachine.Player.transform.localScale.y - hit.distance;

                if (distanceToFloatingPoint == 0f)
                {
                    return;
                }

                float amountToLift = distanceToFloatingPoint * _stateMachine.Player.ResizableCapsuleCollider.SlopeData.StepReachForce - GetPlayerVerticalVelocity().y;

                Vector3 liftForce = new Vector3(0f, amountToLift, 0f);

                _stateMachine.Player.Rigidbody.AddForce(liftForce, ForceMode.VelocityChange);
            }
        }

        private float SetSlopeSpeedModifierOnAngle(float angle)
        {
            float slopeSpeedModifier = _groundedData.SlopeSpeedAngles.Evaluate(angle);

            if (_stateMachine.ReusableData.MovementOnSlopesSpeedModifier != slopeSpeedModifier)
            {
                _stateMachine.ReusableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;

                UpdateCameraRecenteringState(_stateMachine.ReusableData.MovementInput);
            }

            return slopeSpeedModifier;
        }

        private bool IsThereGroundUnderneath()
        {
            PlayerTriggerColliderData triggerColliderData = _stateMachine.Player.ResizableCapsuleCollider.TriggerColliderData;

            Vector3 groundColliderCenterInWorldSpace = triggerColliderData.GroundCheckCollider.bounds.center;

            Collider[] overlappedGroundColliders = Physics.OverlapBox(groundColliderCenterInWorldSpace, triggerColliderData.GroundCheckColliderVerticalExtents, triggerColliderData.GroundCheckCollider.transform.rotation, _stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore);

            return overlappedGroundColliders.Length > 0;
        }
        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            _stateMachine.Player.Input.PlayerActions.Dash.started += OnDashStarted;

            _stateMachine.Player.Input.PlayerActions.Jump.started += OnJumpStarted;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();

            _stateMachine.Player.Input.PlayerActions.Dash.started -= OnDashStarted;

            _stateMachine.Player.Input.PlayerActions.Jump.started -= OnJumpStarted;
        }

        protected virtual void OnMove()
        {
            if (_stateMachine.ReusableData.ShouldSprint)
            {
                _stateMachine.ChangeState(_stateMachine.SprintingState);

                return;
            }

            if (_stateMachine.ReusableData.ShouldWalk)
            {
                _stateMachine.ChangeState(_stateMachine.WalkingState);

                return;
            }

            _stateMachine.ChangeState(_stateMachine.RunningState);
        }

        protected override void OnContactWithGroundExited(Collider collider)
        {
            if (IsThereGroundUnderneath())
            {
                return;
            }

            Vector3 capsuleColliderCenterInWorldSpace = _stateMachine.Player.ResizableCapsuleCollider.CapsuleColliderData.Collider.bounds.center;

            Ray downwardsRayFromCapsuleBottom = new Ray(capsuleColliderCenterInWorldSpace - _stateMachine.Player.ResizableCapsuleCollider.CapsuleColliderData.ColliderVerticalExtents, Vector3.down);

            if (!Physics.Raycast(downwardsRayFromCapsuleBottom, out _, _groundedData.GroundToFallRayDistance, _stateMachine.Player.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                OnFall();
            }
        }

        protected virtual void OnFall() => _stateMachine.ChangeState(_stateMachine.FallingState);

        #region Input Methods

        protected override void OnMovementPerformed(InputAction.CallbackContext context)
        {
            base.OnMovementPerformed(context);

            UpdateTargetRotation(GetMovementInputDirection());
        }
        #endregion

        protected virtual void OnDashStarted(InputAction.CallbackContext context) => _stateMachine.ChangeState(_stateMachine.DashingState);

        protected virtual void OnJumpStarted(InputAction.CallbackContext context) => _stateMachine.ChangeState(_stateMachine.JumpingState);
        #endregion
    }
}