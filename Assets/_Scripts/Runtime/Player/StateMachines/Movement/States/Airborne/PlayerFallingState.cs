using UnityEngine;

namespace RPG.Player.StateMachine
{
    public class PlayerFallingState : PlayerAirborneState
    {
        private Vector3 _playerPositionOnEnter;

        public PlayerFallingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            StartAnimation(_stateMachine.Player.AnimationData.FallParameterHash);

            _stateMachine.ReusableData.MovementSpeedModifier = 0f;

            _playerPositionOnEnter = _stateMachine.Player.transform.position;

            ResetVerticalVelocity();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.Player.AnimationData.FallParameterHash);
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
            Vector3 playerVerticalVelocity = GetPlayerVerticalVelocity();

            if (playerVerticalVelocity.y >= -_airborneData.FallData.FallSpeedLimit)
            {
                return;
            }

            Vector3 limitedVelocityForce = new Vector3(0f, -_airborneData.FallData.FallSpeedLimit - playerVerticalVelocity.y, 0f);

            _stateMachine.Player.Rigidbody.AddForce(limitedVelocityForce, ForceMode.VelocityChange);
        }
        #endregion

        #region Reusable Methods
        protected override void OnContactWithGround(Collider collider)
        {
            float fallDistance = _playerPositionOnEnter.y - _stateMachine.Player.transform.position.y;

            if (fallDistance < _airborneData.FallData.MinimumDistanceToBeConsideredHardFall)
            {
                _stateMachine.ChangeState(_stateMachine.LightLandingState);

                return;
            }

            if (_stateMachine.ReusableData.ShouldWalk && !_stateMachine.ReusableData.ShouldSprint || _stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                _stateMachine.ChangeState(_stateMachine.HardLandingState);

                return;
            }

            _stateMachine.ChangeState(_stateMachine.RollingState);
        }

        protected override void ResetSprintState()
        {
        }
        #endregion
    }
}