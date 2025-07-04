using System.Collections;
using ProjectEmbersteel.Utilities.Inputs.ScriptableObjects;
using UnityEngine;

namespace ProjectEmbersteel.Player.StateMachines.Movement.States
{
    /// <summary>
    /// Class responsible for handling player land state.
    /// </summary>
    public class PlayerLandState : PlayerGroundedState
    {
        // Cached WaitForSeconds used for jump cooldown, used before re-enabling jump
        private readonly WaitForSeconds _jumpDelayWait;

        public PlayerLandState(PlayerStateMachine stateMachine) : base(stateMachine)
        {
            _jumpDelayWait = new WaitForSeconds(_airborneData.JumpData.JumpCooldown);
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            _stateMachine.ReusableData.CurrentJumpForce = _airborneData.JumpData.StationaryForce;

            _stateMachine.PlayerController.StartCoroutine(EnableJumpAfterDelay()); // Default: delay then enable jump
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (_stateMachine.ReusableData.MovementInput == Vector2.zero)
                return;

            OnMove();
        }
        #endregion

        #region Reusable Methods
        public override void OnAnimationTransitionEvent() => _stateMachine.SwitchState(_stateMachine.IdleState);
        #endregion

        #region Main Methods
        // Coroutine that waits for the jump cooldown duration before re-enabling the jump input.
        protected IEnumerator EnableJumpAfterDelay()
        {
            yield return _jumpDelayWait;

            _stateMachine.PlayerController.Input.EnableActionFor(InputActionType.Jump);
        }
        #endregion
    }
}