using UnityEngine;

namespace ProjectEmbersteel.Player.StateMachines.Movement.States
{
    /// <summary>
    /// Class responsible for handling player airborne state.
    /// Base class for jump and fall states.
    /// </summary>
    public class PlayerAirborneState : PlayerBaseState
    {
        private const float GRAVITY = -9.81f;

        public PlayerAirborneState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        #region IState Methods
        public override void Enter()
        {
            _stateMachine.ReusableData.MovementSpeedModifier = 0;

            base.Enter();

            StartAnimation(_stateMachine.PlayerController.AnimationData.AirborneParameterHash);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            _stateMachine.ReusableData.CurrentVerticalVelocity += GRAVITY * _airborneData.GravityMultiplier * Time.deltaTime;

            ApplyMotion();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.PlayerController.AnimationData.AirborneParameterHash);
        }
        #endregion

        #region Reusable Methods
        // Transitions to the land state when the player contacts the ground.
        protected virtual void OnContactWithGround() => _stateMachine.SwitchState(_stateMachine.LandState);
        #endregion
    }
}