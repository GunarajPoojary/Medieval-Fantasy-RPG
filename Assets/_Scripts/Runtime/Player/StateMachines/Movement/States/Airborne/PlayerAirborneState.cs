using UnityEngine;

namespace RPG
{
    public class PlayerAirborneState : PlayerBaseMovementState
    {
        public PlayerAirborneState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            StartAnimation(_stateFactory.PlayerController.AnimationData.AirborneParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateFactory.PlayerController.AnimationData.AirborneParameterHash);
        }
        #endregion

        #region Reusable Methods
        protected override void OnContactWithGround(Collider collider) => _stateFactory.SwitchState(_stateFactory.LightLandState);
        #endregion
    }
}