using UnityEngine; 

namespace RPG.Player.StateMachines.Movement.States.Grounded
{
    /// <summary>
    /// Handles the Idle state of the player while grounded (not moving)
    /// </summary>
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerStateFactory playerStateFactory) : base(playerStateFactory) { }

        #region IState Methods
        public override void Enter()
        {
            // Set the movement speed modifier to the idle value (usually 0)
            _stateFactory.ReusableData.MovementSpeedModifier = _groundedData.IdleData.SpeedModifier;

            base.Enter(); 

            // Set the jump force to stationary jump force (for idle jump behavior)
            _stateFactory.ReusableData.CurrentJumpForce = _airborneData.JumpData.StationaryForce;

            // Completely stop the player's movement
            ResetVelocity();
        }

        public override void UpdateState()
        {
            base.UpdateState();

            // If there's no movement input, remain idle
            if (_stateFactory.ReusableData.MovementInput == Vector2.zero) return;

            OnMove();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            // If the player is not sliding or drifting slightly, do nothing
            if (!IsMovingHorizontally()) return;

            // Otherwise, forcibly stop all horizontal movement
            ResetVelocity();
        }
        #endregion
    }
}