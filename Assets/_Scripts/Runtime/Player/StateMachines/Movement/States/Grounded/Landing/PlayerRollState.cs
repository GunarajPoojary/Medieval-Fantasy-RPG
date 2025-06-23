using UnityEngine; 

namespace ProjectEmbersteel.Player.StateMachines.Movement.States.Grounded.Landing 
{
    /// <summary>
    /// Handles the player's rolling state logic while grounded (after landing from a high fall)
    /// </summary>
    public class PlayerRollState : PlayerGroundedState
    {
        public PlayerRollState(PlayerStateFactory playerStateFactory) : base(playerStateFactory) { }

        #region IState Methods
        public override void Enter()
        {
            // Set the movement speed to roll-specific speed
            _stateFactory.ReusableData.MovementSpeedModifier = _groundedData.RollData.SpeedModifier;

            base.Enter(); 

            StartAnimation(_stateFactory.PlayerController.AnimationData.RollParameterHash);
        }

        public override void Exit()
        {
            base.Exit(); 

            StopAnimation(_stateFactory.PlayerController.AnimationData.RollParameterHash);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate(); 

            // If player is providing movement input, don’t auto-rotate
            if (_stateFactory.ReusableData.MovementInput != Vector2.zero)
                return;

            // If no movement input, rotate to match last known target direction
            RotateTowardsTargetRotation();
        }

        public override void OnAnimationTransitionEvent()
        {
            // If no movement input, go to idle after roll ends
            if (_stateFactory.ReusableData.MovementInput == Vector2.zero)
            {
                _stateFactory.SwitchState(_stateFactory.IdleState);
                return;
            }

            OnMove();
        }
        #endregion

        #region Input Method
        protected override void OnJumpStarted() { }
        #endregion
    }
}