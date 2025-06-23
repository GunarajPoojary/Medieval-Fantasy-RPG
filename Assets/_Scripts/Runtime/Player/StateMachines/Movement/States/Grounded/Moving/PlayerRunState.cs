using UnityEngine; 

namespace ProjectEmbersteel.Player.StateMachines.Movement.States.Grounded.Moving
{
    /// <summary>
    /// Handles the player running state logic while grounded
    /// </summary>
    public class PlayerRunState : PlayerGroundedState
    {
        private float _startTime; 
        private bool _keepRunning; 

        public PlayerRunState(PlayerStateFactory playerStateFactory) : base(playerStateFactory) { }

        #region IState Methods
        public override void Enter()
        {
            // Set movement speed modifier to the configured running speed
            _stateFactory.ReusableData.MovementSpeedModifier = _groundedData.RunData.SpeedModifier;

            base.Enter(); 

            // Set jump force appropriate for medium (running) jumps
            _stateFactory.ReusableData.CurrentJumpForce = _airborneData.JumpData.MediumForce;

            // Save the time the run state was entered
            _startTime = Time.time;

            // If the run input was not held, don't keep running
            if (!_stateFactory.ReusableData.ShouldRun)
                _keepRunning = false;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            // If player still wants to keep running, don't change state
            if (_keepRunning) return;

            // If not enough time has passed, stay in run state for a brief period (RunToWalkTime)
            if (Time.time < _startTime + _groundedData.RunData.RunToWalkTime)
                return;
        }
        #endregion

        #region Main Methods
        // Handles stopping running and transitioning to walk or idle
        private void StopRunning()
        {
            // If there's no movement input, go idle
            if (_stateFactory.ReusableData.MovementInput == Vector2.zero)
            {
                _stateFactory.SwitchState(_stateFactory.IdleState);
                return;
            }

            _stateFactory.SwitchState(_stateFactory.WalkState);
        }
        #endregion

        #region Input Methods
        protected override void OnRun(bool shouldRun)
        {
            base.OnRun(shouldRun); 

            StopRunning();
        }

        protected override void OnMovementCanceled(Vector2 moveInput)
        {
            _stateFactory.SwitchState(_stateFactory.IdleState);

            base.OnMovementCanceled(moveInput);
        }
        #endregion
    }
}