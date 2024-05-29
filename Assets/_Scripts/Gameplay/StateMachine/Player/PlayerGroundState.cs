
using UnityEngine;

namespace GunarajCode
{
    public class PlayerGroundState : PlayerBaseState
    {
        public PlayerGroundState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(currentContext, playerStateFactory)
        {
        }

        public override void CheckSwitchState()
        {
            if (_ctx.IsJumpPressed)
            {
                SwitchState(_factory.Jump());
            }
        }

        public override void EnterState()
        {
            if (_ctx.VerticalVel.y < 0.0f) _ctx.VerticalVel = new Vector3(0, _ctx.GroundedGravity, 0);
        }

        public override void ExitState()
        {

        }

        public override void InitializeSubState()
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateState()
        {
            CheckSwitchState();
        }
    }
}
