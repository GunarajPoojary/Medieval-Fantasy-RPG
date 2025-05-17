using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG
{
    public class PlayerRunState : PlayerGroundedState
    {
        private float _startTime;

        private bool _keepRunning;

        public PlayerRunState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            _stateFactory.ReusableData.MovementSpeedModifier = _groundedData.RunData.SpeedModifier;

            base.Enter();

            _stateFactory.ReusableData.CurrentJumpForce = _airborneData.JumpData.MediumForce;

            _startTime = Time.time;

            if (!_stateFactory.ReusableData.ShouldRun)
            {
                _keepRunning = false;
            }
        }

        public override void Update()
        {
            base.Update();

            if (_keepRunning)
            {
                return;
            }

            if (Time.time < _startTime + _groundedData.RunData.RunToWalkTime)
            {
                return;
            }
        }
        #endregion

        #region Main Methods
        private void StopRunning()
        {
            if (_stateFactory.ReusableData.MovementInput == Vector2.zero)
            {
                _stateFactory.SwitchState(_stateFactory.IdleState);

                return;
            }

            _stateFactory.SwitchState(_stateFactory.WalkState);
        }
        #endregion

        #region Input Methods
        protected override void OnRun(InputAction.CallbackContext ctx)
        {
            base.OnRun(ctx);

            StopRunning();
        }
        protected override void OnMovementCanceled(InputAction.CallbackContext ctx)
        {
            _stateFactory.SwitchState(_stateFactory.IdleState);

            base.OnMovementCanceled(ctx);
        }
        #endregion
    }
}