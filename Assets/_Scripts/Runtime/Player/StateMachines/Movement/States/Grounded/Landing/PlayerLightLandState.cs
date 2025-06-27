using ProjectEmbersteel.Utilities.Inputs.ScriptableObjects;
using UnityEngine; 

namespace ProjectEmbersteel.Player.StateMachines.Movement.States.Grounded.Landing 
{
    /// <summary>
    /// Represents the light landing state — when the player lands softly after a fall or jump
    /// </summary>
    public class PlayerLightLandState : PlayerGroundedState
    {
        public PlayerLightLandState(PlayerStateFactory stateMachine) : base(stateMachine) { }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            // Set jump force to stationary value (typically for idle jump after landing)
            _stateMachine.ReusableData.CurrentJumpForce = _airborneData.JumpData.StationaryForce;

            _stateMachine.PlayerController.Input.DisableActionFor(InputActionType.Jump);

            ResetVelocity();
        }

        public override void Exit()
        {
            base.Exit();

            // After exit, determine if player should transition to idle, walk, or run
            OnLandToMovingState();
        }

        public override void UpdateState()
        {
            base.UpdateState(); 

            if (_stateMachine.ReusableData.MovementInput == Vector2.zero)
                return;

            // If movement input is detected, transition to walk/run
            OnMove();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (!IsMovingHorizontally())
                return;

            ResetVelocity();
        }

        public override void OnAnimationTransitionEvent() => _stateMachine.SwitchState(_stateMachine.IdleState);
        #endregion
    }
}