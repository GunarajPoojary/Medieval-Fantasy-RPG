using UnityEngine;

namespace RPG.Player.StateMachine
{
    public class PlayerIdlingState : PlayerGroundedState
    {
        public PlayerIdlingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            _stateMachine.ReusableData.MovementSpeedModifier = 0f;

            _stateMachine.ReusableData.BackwardsCameraRecenteringData = _groundedData.IdleData.BackwardsCameraRecenteringData;

            base.Enter();

            StartAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);

            _stateMachine.ReusableData.CurrentJumpForce = _airborneData.JumpData.StationaryForce;

            ResetVelocity();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.Player.AnimationData.IdleParameterHash);
        }

        public override void Update()
        {
            base.Update();

            if (_stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                return;
            }

            OnMove();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (!IsMovingHorizontally())
            {
                return;
            }

            ResetVelocity();
        }
        #endregion
    }
}