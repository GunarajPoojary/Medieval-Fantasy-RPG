using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Player.StateMachine
{
    public class PlayerSprintingState : PlayerMovingState
    {
        private float _startTime;

        private bool _keepSprinting;
        private bool _shouldResetSprintState;

        public PlayerSprintingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            _stateMachine.ReusableData.MovementSpeedModifier = _groundedData.SprintData.SpeedModifier;

            base.Enter();

            StartAnimation(_stateMachine.Player.AnimationData.SprintParameterHash);

            _stateMachine.ReusableData.CurrentJumpForce = _airborneData.JumpData.StrongForce;

            _startTime = Time.time;

            _shouldResetSprintState = true;

            if (!_stateMachine.ReusableData.ShouldSprint)
            {
                _keepSprinting = false;
            }
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.Player.AnimationData.SprintParameterHash);

            if (_shouldResetSprintState)
            {
                _keepSprinting = false;

                _stateMachine.ReusableData.ShouldSprint = false;
            }
        }

        public override void Update()
        {
            base.Update();

            if (_keepSprinting)
            {
                return;
            }

            if (Time.time < _startTime + _groundedData.SprintData.SprintToRunTime)
            {
                return;
            }

            StopSprinting();
        }
        #endregion

        #region Main Methods
        private void StopSprinting()
        {
            if (_stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                _stateMachine.ChangeState(_stateMachine.IdlingState);

                return;
            }

            _stateMachine.ChangeState(_stateMachine.RunningState);
        }
        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            _stateMachine.Player.Input.PlayerActions.Sprint.performed += OnSprintPerformed;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();

            _stateMachine.Player.Input.PlayerActions.Sprint.performed -= OnSprintPerformed;
        }

        protected override void OnFall()
        {
            _shouldResetSprintState = false;

            base.OnFall();
        }
        #endregion

        #region Input Methods
        private void OnSprintPerformed(InputAction.CallbackContext context)
        {
            _keepSprinting = true;

            _stateMachine.ReusableData.ShouldSprint = true;
        }

        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            _stateMachine.ChangeState(_stateMachine.HardStoppingState);

            base.OnMovementCanceled(context);
        }

        protected override void OnJumpStarted(InputAction.CallbackContext context)
        {
            _shouldResetSprintState = false;

            base.OnJumpStarted(context);
        }
        #endregion
    }
}