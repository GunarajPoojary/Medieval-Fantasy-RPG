using UnityEngine;

namespace RPG
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            StateFactory.ReusableData.MovementSpeedModifier = _groundedData.IdleData.SpeedModifier;

            base.Enter();

            StateFactory.ReusableData.CurrentJumpForce = _airborneData.JumpData.StationaryForce;

            ResetVelocity();
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
        #endregion
    }
}