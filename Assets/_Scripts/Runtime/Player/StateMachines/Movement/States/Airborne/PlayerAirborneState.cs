using UnityEngine;

namespace RPG.Player.StateMachine
{
    public class PlayerAirborneState : PlayerMovementState
    {
        public PlayerAirborneState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            StartAnimation(_stateMachine.Player.AnimationData.AirborneParameterHash);

            ResetSprintState();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.Player.AnimationData.AirborneParameterHash);
        }
        #endregion

        #region Reusable Methods
        protected virtual void ResetSprintState() => _stateMachine.ReusableData.ShouldSprint = false;

        protected override void OnContactWithGround(Collider collider) => _stateMachine.ChangeState(_stateMachine.LightLandingState);
        #endregion
    }
}