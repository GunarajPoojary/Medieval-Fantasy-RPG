using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG
{
    public class PlayerRollState : PlayerGroundedState
    {
        public PlayerRollState(PlayerStateFactory playerStateFactory) : base(playerStateFactory)
        {
        }

        #region IState Methods
        public override void Enter()
        {
            StateFactory.ReusableData.MovementSpeedModifier = _groundedData.RollData.SpeedModifier;

            base.Enter();

            StartAnimation(StateFactory.PlayerController.AnimationData.RollParameterHash);
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(StateFactory.PlayerController.AnimationData.RollParameterHash);
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if (StateFactory.ReusableData.MovementInput != Vector2.zero)
            {
                return;
            }

            RotateTowardsTargetRotation();
        }

        public override void OnAnimationTransitionEvent()
        {
            if (StateFactory.ReusableData.MovementInput == Vector2.zero)
            {
                StateFactory.ChangeState(StateFactory.IdleState);

                return;
            }

            OnMove();
        }
        #endregion

        #region Input Method
        protected override void OnJumpStarted(InputAction.CallbackContext context)
        {
        }
        #endregion
    }
}