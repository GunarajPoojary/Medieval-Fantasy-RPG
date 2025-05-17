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
            _stateFactory.ReusableData.MovementSpeedModifier = _groundedData.WalkData.SpeedModifier;

            base.Enter();

            _stateFactory.ReusableData.CurrentJumpForce = _airborneData.JumpData.WeakForce;
        }
        #endregion

        #region Input Methods
        protected override void OnMovementCanceled(InputAction.CallbackContext context)
        {
            _stateFactory.SwitchState(_stateFactory.IdleState);

            base.OnMovementCanceled(context);
        }
        #endregion
    }
}