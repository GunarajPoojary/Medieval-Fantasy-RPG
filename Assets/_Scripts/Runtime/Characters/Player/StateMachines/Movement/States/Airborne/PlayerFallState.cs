using UnityEngine;

namespace ProjectEmbersteel.Player.StateMachines.Movement.States
{
    /// <summary>
    /// Class responsible for handling player fall state.
    /// </summary>
    public class PlayerFallState : PlayerAirborneState
    {
        private const float RAY_HIT_DISTANCE_THRESHOLD = 0.01f;
        private const float RAY_MAX_DISTANCE = 1f;

        public PlayerFallState(PlayerStateMachine stateMachine) : base(stateMachine) { }

        #region IState Methods
        public override void Enter()
        {
            base.Enter();

            StartAnimation(_stateMachine.PlayerController.AnimationData.FallParameterHash);
        }

        public override void UpdateState()
        {
            base.UpdateState();

            CheckForGrounded();
        }

        public override void Exit()
        {
            base.Exit();

            StopAnimation(_stateMachine.PlayerController.AnimationData.FallParameterHash);
        }
        #endregion

        #region Main Methods
        // Performs a raycast downward to detect if the player has landed on the ground.
        // If the ray hits within a small threshold, it considers the player grounded.
        private void CheckForGrounded()
        {
            if (Physics.Raycast(
                _stateMachine.PlayerController.transform.position,
                Vector3.down,
                out RaycastHit hit,
                RAY_MAX_DISTANCE,
                _stateMachine.PlayerController.LayerData.GroundLayer,
                QueryTriggerInteraction.Ignore))
            {
                if (hit.distance < RAY_HIT_DISTANCE_THRESHOLD)
                    OnContactWithGround(); // Handles transition to grounded state
            }
        }
        #endregion
    }
}