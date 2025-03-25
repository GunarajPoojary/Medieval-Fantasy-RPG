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

            StateFactory.ReusableData.CurrentJumpForce = _airborneData.JumpData.StationaryForce;

            StateFactory.PlayerController.Input.PlayerActions.Jump.Disable();

            ResetVelocity();
        }

        public override void Exit()
        {
            base.Exit();

            StateFactory.PlayerController.Input.PlayerActions.Jump.Enable();
        }

        public override void Update()
        {
            base.Update();

            if (StateFactory.ReusableData.MovementInput == Vector2.zero)
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

        public override void OnAnimationTransitionEvent() => StateFactory.ChangeState(StateFactory.IdleState);
        #endregion
    }
}