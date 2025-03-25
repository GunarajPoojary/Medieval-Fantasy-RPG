using UnityEngine;

namespace RPG
{
    public class PlayerAirborneState : PlayerMovementState
    {
        public PlayerAirborneState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            StartAnimation(StateFactory.PlayerController.AnimationData.AirborneParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(StateFactory.PlayerController.AnimationData.AirborneParameterHash);
        }
        #endregion

        #region Reusable Methods
        protected override void OnContactWithGround(Collider collider) => StateFactory.ChangeState(StateFactory.LightLandState);
        #endregion
    }
}