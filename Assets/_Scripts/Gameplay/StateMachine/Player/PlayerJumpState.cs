using UnityEngine;

namespace GunarajCode
{
    public class PlayerJumpState : PlayerBaseState
    {
        public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
        {
        }

        public override void CheckSwitchState()
        {
            if (_ctx.IsGrounded)
            {
                SwitchState(_factory.Grounded());
            }
        }

        public override void EnterState()
        {
            HandleJump();
        }

        public override void ExitState()
        {
            _ctx.IsJumping = false;
            _ctx.Animator.SetBool(_ctx.IsJumpingHash, false);
            _ctx.Animator.SetBool(_ctx.IsFallingHash, false);
        }

        public override void InitializeSubState()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateState()
        {
            CheckSwitchState();
            HandleGravity();
        }

        void HandleJump()
        {
            if (_ctx.Timer <= 0.0f)
            {
                _ctx.Timer = _ctx.CooldownTime;
                _ctx.IsJumping = true;

                _ctx.Animator.SetBool(_ctx.IsJumpingHash, true);

                // Calculate jump velocity
                _ctx.VerticalVel = new Vector3(0, Mathf.Sqrt(-2 * Physics.gravity.y * _ctx.JumpHeight), 0);
            }
        }

        void HandleGravity()
        {
            if (_ctx.VerticalVel.y < 0.0f)
            {
                _ctx.IsJumping = false;

                //_ctx.Animator.SetBool(_ctx.IsJumpingHash, false);
                _ctx.Animator.SetBool(_ctx.IsFallingHash, true);
            }

            _ctx.VerticalVel += new Vector3(0, Physics.gravity.y * 2 * Time.deltaTime, 0);

            _ctx.Controller.Move(_ctx.VerticalVel * Time.deltaTime);
        }
    }
}
