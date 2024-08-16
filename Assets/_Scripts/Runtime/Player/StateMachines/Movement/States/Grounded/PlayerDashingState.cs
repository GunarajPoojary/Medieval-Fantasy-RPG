using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.Player.StateMachine
{
    public class PlayerDashingState : PlayerGroundedState
    {
        private float _startTime;

        private int _consecutiveDashesUsed;

        private bool _shouldKeepRotating;

        public PlayerDashingState(PlayerMovementStateMachine playerMovementStateMachine) : base(playerMovementStateMachine)
        {
        }


        #region IState Methods
        public override void Enter()
        {
            _stateMachine.ReusableData.MovementSpeedModifier = _groundedData.DashData.SpeedModifier;

            base.Enter();

            StartAnimation(_stateMachine.Player.AnimationData.DashParameterHash);

            _stateMachine.ReusableData.CurrentJumpForce = _airborneData.JumpData.StrongForce;

            _stateMachine.ReusableData.RotationData = _groundedData.DashData.RotationData;

            Dash();

            _shouldKeepRotating = _stateMachine.ReusableData.MovementInput != Vector2.zero;

            UpdateConsecutiveDashes();

            _startTime = Time.time;
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.Player.AnimationData.DashParameterHash);

            SetBaseRotationData();
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (!_shouldKeepRotating)
            {
                return;
            }

            RotateTowardsTargetRotation();
        }

        public override void OnAnimationTransitionEvent()
        {
            if (_stateMachine.ReusableData.MovementInput == Vector2.zero)
            {
                _stateMachine.ChangeState(_stateMachine.HardStoppingState);

                return;
            }

            _stateMachine.ChangeState(_stateMachine.SprintingState);
        }
        #endregion

        #region Main Methods
        private void Dash()
        {
            Vector3 dashDirection = _stateMachine.Player.transform.forward;

            dashDirection.y = 0f;

            UpdateTargetRotation(dashDirection, false);

            if (_stateMachine.ReusableData.MovementInput != Vector2.zero)
            {
                UpdateTargetRotation(GetMovementInputDirection());

                dashDirection = GetTargetRotationDirection(_stateMachine.ReusableData.CurrentTargetRotation.y);
            }

            _stateMachine.Player.Rigidbody.velocity = dashDirection * GetMovementSpeed(false);
        }

        private void UpdateConsecutiveDashes()
        {
            if (!IsConsecutive())
            {
                _consecutiveDashesUsed = 0;
            }

            ++_consecutiveDashesUsed;

            if (_consecutiveDashesUsed == _groundedData.DashData.ConsecutiveDashesLimitAmount)
            {
                _consecutiveDashesUsed = 0;

                _stateMachine.Player.Input.DisableActionFor(_stateMachine.Player.Input.PlayerActions.Dash, _groundedData.DashData.DashLimitReachedCooldown);
            }
        }

        private bool IsConsecutive() => Time.time < _startTime + _groundedData.DashData.TimeToBeConsideredConsecutive;
        #endregion

        #region Reusable Methods
        protected override void AddInputActionsCallbacks()
        {
            base.AddInputActionsCallbacks();

            _stateMachine.Player.Input.PlayerActions.Movement.performed += OnMovementPerformed;
        }

        protected override void RemoveInputActionsCallbacks()
        {
            base.RemoveInputActionsCallbacks();

            _stateMachine.Player.Input.PlayerActions.Movement.performed -= OnMovementPerformed;
        }
        #endregion

        #region Input Methods
        protected override void OnMovementPerformed(InputAction.CallbackContext context)
        {
            base.OnMovementPerformed(context);

            _shouldKeepRotating = true;
        }

        protected override void OnDashStarted(InputAction.CallbackContext context)
        {
        }
        #endregion
    }
}