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
            StateFactory.ReusableData.MovementSpeedModifier = _groundedData.RunData.SpeedModifier;

            base.Enter();

            StateFactory.ReusableData.CurrentJumpForce = _airborneData.JumpData.MediumForce;

            _startTime = Time.time;

            if (!StateFactory.ReusableData.ShouldRun)
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
            if (StateFactory.ReusableData.MovementInput == Vector2.zero)
            {
                StateFactory.ChangeState(StateFactory.IdleState);

                return;
            }

            StateFactory.ChangeState(StateFactory.WalkState);
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
            StateFactory.ChangeState(StateFactory.IdleState);

            base.OnMovementCanceled(ctx);
        }
        #endregion
    }
}