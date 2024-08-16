using UnityEngine.InputSystem;

namespace RPG.Player.StateMachine
{
    public class PlayerWalkingState : PlayerMovingState
    {
        public PlayerWalkingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            _stateMachine.ReusableData.MovementSpeedModifier = _groundedData.WalkData.SpeedModifier;

            _stateMachine.ReusableData.BackwardsCameraRecenteringData = _groundedData.WalkData.BackwardsCameraRecenteringData;

            base.Enter();

            StartAnimation(_stateMachine.Player.AnimationData.WalkParameterHash);

            _stateMachine.ReusableData.CurrentJumpForce = _airborneData.JumpData.WeakForce;
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.Player.AnimationData.WalkParameterHash);

            SetBaseCameraRecenteringData();
        }
        #endregion

        #region Input Methods
        protected override void OnWalkToggleStarted(InputAction.CallbackContext context)
        {
            base.OnWalkToggleStarted(context);

            _stateMachine.ChangeState(_stateMachine.RunningState);
        }

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            _stateMachine.ChangeState(_stateMachine.LightStoppingState);

            base.OnMovementCanceled(context);
        }
        #endregion
    }
}