using UnityEngine;

namespace RPG
{
    public class PlayerFallState : PlayerAirborneState
    {
        private Vector3 _playerPositionOnEnter;

        public PlayerFallState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            StartAnimation(StateFactory.PlayerController.AnimationData.FallParameterHash);

            _playerPositionOnEnter = StateFactory.PlayerController.transform.position;

            ResetVerticalVelocity();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(StateFactory.PlayerController.AnimationData.FallParameterHash);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            LimitVerticalVelocity();
        }
        #endregion

        #region Main Methods
        private void LimitVerticalVelocity()
        {
            Vector3 playerVerticalVelocity = GetVerticalVelocity();

            if (playerVerticalVelocity.y >= -_airborneData.FallData.FallSpeedLimit)
            {
                return;
            }

            Vector3 limitedVelocityForce = new(0f, -_airborneData.FallData.FallSpeedLimit - playerVerticalVelocity.y, 0f);

            StateFactory.PlayerController.Rigidbody.AddForce(limitedVelocityForce, ForceMode.VelocityChange);
        }
        #endregion

        #region Reusable Methods
        protected override void OnContactWithGround(Collider collider)
        {
            float fallDistance = _playerPositionOnEnter.y - StateFactory.PlayerController.transform.position.y;

            if (fallDistance < _airborneData.FallData.MinimumDistanceToBeConsideredHardFall)
            {
                StateFactory.ChangeState(StateFactory.LightLandState);

                return;
            }

            if (!StateFactory.ReusableData.ShouldRun || StateFactory.ReusableData.MovementInput == Vector2.zero)
            {
                StateFactory.ChangeState(StateFactory.HardLandState);

                return;
            }

            StateFactory.ChangeState(StateFactory.RollState);
        }
        #endregion
    }
}