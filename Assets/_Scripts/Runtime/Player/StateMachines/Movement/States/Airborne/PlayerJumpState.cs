using UnityEngine; 

namespace RPG.Player.StateMachines.Movement.States.Airborne
{
    /// <summary>
    /// Handles the player's jumping state
    /// </summary>
    public class PlayerJumpState : PlayerAirborneState
    {
        private bool _shouldKeepRotating; 
        private bool _canStartFalling; 

        public PlayerJumpState(PlayerStateFactory playerStateFactory) : base(playerStateFactory) { }

        #region IState Methods
        public override void Enter()
        {
            base.Enter(); 

            // Set deceleration force while jumping (to slow vertical velocity)
            _stateFactory.ReusableData.MovementDecelerationForce = _airborneData.JumpData.DecelerationForce;

            // Override rotation data specifically for jump
            _stateFactory.ReusableData.RotationData = _airborneData.JumpData.RotationData;

            // Determine if the player should keep rotating mid-jump (only if input exists)
            _shouldKeepRotating = _stateFactory.ReusableData.MovementInput != Vector2.zero;

            Jump(); 
        }

        public override void Exit()
        {
            base.Exit(); 

            SetBaseRotationData();

            _canStartFalling = false;
        }

        public override void UpdateState()
        {
            base.UpdateState(); 

            // Wait until character has started moving upward before enabling fall transition
            if (!_canStartFalling && IsMovingUp(0f)) 
                _canStartFalling = true;

            // If not ready to fall or still moving up, skip
            if (!_canStartFalling || IsMovingUp(0f)) 
                return;

            _stateFactory.SwitchState(_stateFactory.FallState);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            // Keep rotating toward input direction if applicable
            if (_shouldKeepRotating) 
                RotateTowardsTargetRotation();

            // Apply deceleration if still moving up
            if (IsMovingUp()) 
                DecelerateVertically();
        }
        #endregion

        #region Main Methods
        private void Jump()
        {
            // Get jump force vector based on current data
            Vector3 jumpForce = _stateFactory.ReusableData.CurrentJumpForce;

            // Default jump direction is forward (in local space)
            Vector3 jumpDirection = _stateFactory.PlayerController.transform.forward;

            if (_shouldKeepRotating)
            {
                // Update rotation to align with movement direction
                UpdateTargetRotation(GetMovementInputDirection());

                // Adjust direction based on camera-relative movement input
                jumpDirection = GetTargetRotationDirection(_stateFactory.ReusableData.CurrentTargetRotation.y);
            }

            // Multiply horizontal force by directional values
            jumpForce.x *= jumpDirection.x;
            jumpForce.z *= jumpDirection.z;

            // Adjust jump force based on slope detection
            jumpForce = GetJumpForceOnSlope(jumpForce);

            ResetVelocity(); 

            _stateFactory.PlayerController.Rigidbody.AddForce(jumpForce, ForceMode.VelocityChange);
        }

        // Modifies the jump force based on ground slope beneath player
        private Vector3 GetJumpForceOnSlope(Vector3 jumpForce)
        {
            Vector3 capsuleColliderCenterInWorldSpace = _stateFactory.PlayerController.ResizableCapsuleCollider.CapsuleColliderData.Collider.bounds.center;

            // Create a ray straight down from the player's position
            Ray downwardsRayFromCapsuleCenter = new(capsuleColliderCenterInWorldSpace, Vector3.down);

            // Cast a ray to detect slope below
            if (Physics.Raycast(
                    downwardsRayFromCapsuleCenter,
                    out RaycastHit hit,
                    _airborneData.JumpData.JumpToGroundRayDistance,
                    _stateFactory.PlayerController.LayerData.GroundLayer,
                    QueryTriggerInteraction.Ignore))
            {
                // Get the angle between ground normal and downward direction
                float groundAngle = Vector3.Angle(hit.normal, -downwardsRayFromCapsuleCenter.direction);

                if (IsMovingUp())
                {
                    // Reduce jump force when jumping uphill
                    float forceModifier = _airborneData.JumpData.JumpForceModifierOnSlopeUpwards.Evaluate(groundAngle);
                    jumpForce.x *= forceModifier;
                    jumpForce.z *= forceModifier;
                }

                if (IsMovingDown())
                {
                    // Modify vertical jump force when falling off slope
                    float forceModifier = _airborneData.JumpData.JumpForceModifierOnSlopeDownwards.Evaluate(groundAngle);
                    jumpForce.y *= forceModifier;
                }
            }

            return jumpForce; 
        }
        #endregion

        #region Input Method
        protected override void OnMovementCanceled(Vector2 moveInput) { }
        #endregion
    }
}