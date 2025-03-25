using UnityEngine.InputSystem;

namespace RPG
{
    public class PlayerWalkState : PlayerGroundedState
    {
        public PlayerWalkState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            StateFactory.ReusableData.MovementSpeedModifier = _groundedData.WalkData.SpeedModifier;

            base.Enter();

            StateFactory.ReusableData.CurrentJumpForce = _airborneData.JumpData.WeakForce;
        }
        #endregion

        #region Input Methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            StateFactory.ChangeState(StateFactory.IdleState);

            base.OnMovementCanceled(context);
        }
        #endregion
    }
}