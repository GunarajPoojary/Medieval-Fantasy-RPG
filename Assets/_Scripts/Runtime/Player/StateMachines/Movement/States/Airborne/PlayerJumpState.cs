using ProjectEmbersteel.Utilities.Inputs.ScriptableObjects;
using UnityEngine;

namespace ProjectEmbersteel.Player.StateMachines.Movement.States
{
    /// <summary>
    /// Class responsible for handling player jump state.
    /// </summary>
    public class PlayerJumpState : PlayerAirborneState
    {
        private float _previousVerticalVelocity;

        public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            _stateMachine.PlayerController.Input.DisableActionFor(InputActionType.Jump);

            Jump();

            _previousVerticalVelocity = _stateMachine.PlayerController.Controller.velocity.y;
        }

        public override void UpdateState()
        {
            base.UpdateState();

            CheckForFall();
        }
        #endregion

        #region Main Methods
        // Checks if the player has has reached max vertical velocity.
        // If vertical velocity drops, the player is now falling and transitions to the fall state.
        private void CheckForFall()
        {
            if (_previousVerticalVelocity > _stateMachine.PlayerController.Controller.velocity.y)
            {
                _stateMachine.SwitchState(_stateMachine.FallState);
                return;
            }

            _previousVerticalVelocity = _stateMachine.PlayerController.Controller.velocity.y;
        }

        // Sets the vertical velocity based on the jump force data from the state machine.
        // Note: The velocity applied to character controller in the PlayerBaseState class in the UpdateState Method
        private void Jump()
        {
            Vector3 jumpForce = _stateMachine.ReusableData.CurrentJumpForce;

            _stateMachine.ReusableData.CurrentVerticalVelocity = jumpForce.y;
        }
        #endregion
    }
}