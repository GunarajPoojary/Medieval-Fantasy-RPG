using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Player.StateMachine
{
    public class PlayerRunningState : PlayerMovingState
    {
        private float _startTime;

        public PlayerRunningState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            _stateMachine.ReusableData.MovementSpeedModifier = _groundedData.RunData.SpeedModifier;

            base.Enter();

            StartAnimation(_stateMachine.Player.AnimationData.RunParameterHash);

            _stateMachine.ReusableData.CurrentJumpForce = _airborneData.JumpData.MediumForce;

            _startTime = Time.time;
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.Player.AnimationData.RunParameterHash);
        }

        public override void Update()
        {
            base.Update();

            if (!_stateMachine.ReusableData.ShouldWalk)
            {
                return;
            }

            if (Time.time < _startTime + _groundedData.SprintData.RunToWalkTime)
            {
                return;
            }

            StopRunning();
        }
        #endregion

        #region Main Methods
        private void StopRunning()
        {
            if (_stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                _stateMachine.ChangeState(_stateMachine.IdlingState);

                return;
            }

            _stateMachine.ChangeState(_stateMachine.WalkingState);
        }
        #endregion

        #region Input Methods
        protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            base.OnWalkToggleStarted(context);

            _stateMachine.ChangeState(_stateMachine.WalkingState);
        }

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            _stateMachine.ChangeState(_stateMachine.MediumStoppingState);

            base.OnMovementCanceled(context);
        }
        #endregion
    }
}