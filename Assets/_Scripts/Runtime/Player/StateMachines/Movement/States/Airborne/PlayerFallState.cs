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

            StartAnimation(_stateFactory.PlayerController.AnimationData.FallParameterHash);

            _playerPositionOnEnter = _stateFactory.PlayerController.transform.position;

            ResetVerticalVelocity();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateFactory.PlayerController.AnimationData.FallParameterHash);
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

            _stateFactory.PlayerController.Rigidbody.AddForce(limitedVelocityForce, ForceMode.VelocityChange);
        }
        #endregion

        #region Reusable Methods
        protected override void OnContactWithGround(Collider collider)
        {
            float fallDistance = _playerPositionOnEnter.y - _stateFactory.PlayerController.transform.position.y;

            if (fallDistance < _airborneData.FallData.MinimumDistanceToBeConsideredHardFall)
            {
                _stateFactory.SwitchState(_stateFactory.LightLandState);

                return;
            }

            if (!_stateFactory.ReusableData.ShouldRun || _stateFactory.ReusableData.MovementInput == Vector2.zero)
            {
                _stateFactory.SwitchState(_stateFactory.HardLandState);

                return;
            }

            _stateFactory.SwitchState(_stateFactory.RollState);
        }
        #endregion
    }
}