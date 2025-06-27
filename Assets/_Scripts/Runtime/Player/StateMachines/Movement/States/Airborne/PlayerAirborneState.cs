using UnityEngine; 

namespace ProjectEmbersteel.Player.StateMachines.Movement.States.Airborne
{
    /// <summary>
    /// Handles the common airborne (in-air) state logics of the player.
    /// </summary>
    public class PlayerAirborneState : PlayerBaseMovementState
    {
        public PlayerAirborneState(PlayerStateFactory stateMachine) : base(stateMachine) { }

        #region IState Methods
        public override void Enter()
        {
            _stateMachine.ReusableData.MovementSpeedModifier = 0;
            base.Enter(); 

            StartAnimation(_stateMachine.PlayerController.AnimationData.AirborneParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.PlayerController.AnimationData.AirborneParameterHash);
        }
        #endregion

        #region Reusable Methods
        protected override void OnContactWithGround(Collider collider) => _stateMachine.SwitchState(_stateMachine.LightLandState);
        #endregion
    }
}