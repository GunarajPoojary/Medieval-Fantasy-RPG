using System.Collections;
using RPG.Player.Data.Colliders;
using RPG.Utilities.Inputs.ScriptableObjects;
using UnityEngine;

namespace RPG.Player.StateMachines.Movement.States.Grounded
{
    /// <summary>
    /// Handles common movement logic related to the grounded state of the player
    /// </summary>
    public class PlayerGroundedState : PlayerBaseMovementState
    {
        private WaitForSeconds _jumpDelayWait;
        private readonly Collider[] _groundCheckResults = new Collider[2];

        public PlayerGroundedState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
        {
            _jumpDelayWait = new WaitForSeconds(_groundedData.JumpDelay);
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

        // Ensures ShouldRun is false if the player is not providing movement input
        private void UpdateShouldRunState()
        {
            if (!_stateFactory.ReusableData.ShouldRun) return;
            if (_stateFactory.ReusableData.MovementInput != Vector2.zero) return;

            _stateFactory.ReusableData.ShouldRun = false;
        }
        #endregion

        #region Main Methods
        // Applies a "float" force to keep the player hovering correctly over sloped terrain
        private void Float()
        {
            // Get collider center in world space
            Vector3 capsuleColliderCenterInWorldSpace = _stateFactory.PlayerController.ResizableCapsuleCollider.CapsuleColliderData.Collider.bounds.center;

            // Cast a ray down from the center of the capsule
            Ray downwardsRayFromCapsuleCenter = new(capsuleColliderCenterInWorldSpace, Vector3.down);

            // Check for ground hit using the float ray
            if (Physics.Raycast(downwardsRayFromCapsuleCenter, out RaycastHit hit,
                _stateFactory.PlayerController.ResizableCapsuleCollider.SlopeData.FloatRayDistance,
                _stateFactory.PlayerController.LayerData.GroundLayer, QueryTriggerInteraction.Ignore))
            {
                // Calculate ground angle for slope detection
                float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

                // Modify movement speed based on slope steepness
                float slopeSpeedModifier = SetSlopeSpeedModifierOnAngle(groundAngle);

                if (slopeSpeedModifier == 0f) return; // Can't move on this slope

                // Calculate how far off the ground we are
                float distanceToFloatingPoint =
                    _stateFactory.PlayerController.ResizableCapsuleCollider.CapsuleColliderData.ColliderCenterInLocalSpace.y *
                    _stateFactory.PlayerController.transform.localScale.y - hit.distance;

                if (distanceToFloatingPoint == 0f) return; // Already aligned with ground

                // Calculate upward force needed to match ground height
                float amountToLift = distanceToFloatingPoint *
                    _stateFactory.PlayerController.ResizableCapsuleCollider.SlopeData.StepReachForce - GetVerticalVelocity().y;

                // Apply vertical lift force
                Vector3 liftForce = new Vector3(0f, amountToLift, 0f);

                _stateFactory.PlayerController.Rigidbody.AddForce(liftForce, ForceMode.VelocityChange);
            }
        }

        // Sets slope-based movement speed modifier
        private float SetSlopeSpeedModifierOnAngle(float angle)
        {
            float slopeSpeedModifier = _groundedData.SlopeSpeedAngles.Evaluate(angle);

            if (_stateFactory.ReusableData.MovementOnSlopesSpeedModifier != slopeSpeedModifier)
            {
                _stateFactory.ReusableData.MovementOnSlopesSpeedModifier = slopeSpeedModifier;
            }

            return slopeSpeedModifier;
        }

        // Checks if there's still ground under the player (used to confirm grounded state)
        private bool IsThereGroundUnderneath()
        {
            var triggerColliderData = _stateFactory.PlayerController.ResizableCapsuleCollider.TriggerColliderData;

            Vector3 center = triggerColliderData.GroundCheckCollider.bounds.center;
            Vector3 halfExtents = triggerColliderData.GroundCheckColliderVerticalExtents;
            Quaternion orientation = triggerColliderData.GroundCheckCollider.transform.rotation;
            int layerMask = _stateFactory.PlayerController.LayerData.GroundLayer;

            int count = Physics.OverlapBoxNonAlloc(
                center,
                halfExtents,
                _groundCheckResults,
                orientation,
                layerMask,
                QueryTriggerInteraction.Ignore
            );

            return count > 0;
        }
        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();
            _stateFactory.PlayerController.Input.JumpStartedAction += OnJumpStarted;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();
            _stateFactory.PlayerController.Input.JumpStartedAction -= OnJumpStarted;
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
            if (IsThereGroundUnderneath()) return;

            Vector3 capsuleColliderCenterInWorldSpace = _stateFactory.PlayerController.ResizableCapsuleCollider.CapsuleColliderData.Collider.bounds.center;

            Ray downwardsRayFromCapsuleBottom = new(
                capsuleColliderCenterInWorldSpace - _stateFactory.PlayerController.ResizableCapsuleCollider.CapsuleColliderData.ColliderVerticalExtents,
                Vector3.down
            );

            if (!Physics.Raycast(
                downwardsRayFromCapsuleBottom, out _,
                _groundedData.GroundToFallRayDistance,
                _stateFactory.PlayerController.LayerData.GroundLayer,
                QueryTriggerInteraction.Ignore)) OnFall();
        }

        protected virtual void OnFall() => _stateFactory.SwitchState(_stateFactory.FallState);

        // Called when landing from a jump — chooses walk/run/idle depending on input
        protected void OnLandToMovingState()
        {
            if (_stateFactory.ReusableData.MovementInput == Vector2.zero)
            {
                _stateFactory.PlayerController.Input.EnableActionFor(InputActionType.Jump); // No movement input, just enable jump
                return;
            }

            if (_stateFactory.ReusableData.ShouldRun)
            {
                _stateFactory.PlayerController.StartCoroutine(EnableJumpAfterDelay()); // Delay jump after landing
                return;
            }

            _stateFactory.PlayerController.StartCoroutine(EnableJumpAfterDelay()); // Default: delay then enable jump
        }

        // Coroutine to enable jump input after a short delay
        private IEnumerator EnableJumpAfterDelay()
        {
            yield return _jumpDelayWait;
            _stateFactory.PlayerController.Input.EnableActionFor(InputActionType.Jump); // Then enable jump
        }
        #endregion

        #region Input Methods
        protected override void OnRun(bool shouldRun)
        {
            base.OnRun(shouldRun);

            if (_stateFactory.ReusableData.MovementInput != Vector2.zero) OnMove();
        }

        protected override void OnMovementPerformed(Vector2 moveInput)
        {
            base.OnMovementPerformed(moveInput);

            // Update facing direction when movement starts
            UpdateTargetRotation(GetMovementInputDirection());
        }

        protected virtual void OnJumpStarted() => _stateFactory.SwitchState(_stateFactory.JumpState);
        #endregion
    }
}