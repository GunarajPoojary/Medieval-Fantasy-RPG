using UnityEngine; 

namespace ProjectEmbersteel.Player.StateMachines.Movement.States.Grounded.Landing 
{
    /// <summary>
    /// Handles the player's rolling state logic while grounded (after landing from a high fall)
    /// </summary>
    public class PlayerRollState : PlayerGroundedState
    {
        public PlayerRollState(PlayerStateFactory stateMachine) : base(stateMachine) { }

        #region IState Methods
        public override void Enter()
        {
            // Set the movement speed to roll-specific speed
            _stateMachine.ReusableData.MovementSpeedModifier = _groundedData.RollData.SpeedModifier;

            base.Enter(); 

            StartAnimation(_stateMachine.PlayerController.AnimationData.RollParameterHash);
        }

        public override void Exit()
        {
            base.Exit(); 

            StopAnimation(_stateMachine.PlayerController.AnimationData.RollParameterHash);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate(); 

            // If player is providing movement input, don’t auto-rotate
            if (_stateMachine.ReusableData.MovementInput != Vector2.zero)
                return;

            // If no movement input, rotate to match last known target direction
            RotateTowardsTargetRotation();
        }

        public override void OnAnimationTransitionEvent()
        {
            // If no movement input, go to idle after roll ends
            if (_stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                _stateMachine.SwitchState(_stateMachine.IdleState);
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