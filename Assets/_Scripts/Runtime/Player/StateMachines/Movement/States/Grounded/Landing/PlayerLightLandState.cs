using UnityEngine;

namespace RPG
{
    public class PlayerLightLandState : PlayerGroundedState
    {
        public PlayerLightLandState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            _stateFactory.ReusableData.CurrentJumpForce = _airborneData.JumpData.StationaryForce;

            _stateFactory.PlayerController.Input.PlayerActions.Jump.Disable();

            ResetVelocity();
        }

        public override void Exit()
        {
            base.Exit();

            _stateFactory.PlayerController.Input.PlayerActions.Jump.Enable();
        }

        public override void Update()
        {
            base.Update();

            if (_stateFactory.ReusableData.MovementInput == Vector2.zero)
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

        public override void OnAnimationTransitionEvent() => _stateFactory.SwitchState(_stateFactory.IdleState);
        #endregion
    }
}