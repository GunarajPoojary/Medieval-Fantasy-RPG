using ProjectEmbersteel.Player.Data.States;
using ProjectEmbersteel.Player.StateMachines.Movement.States;

namespace ProjectEmbersteel.Player.StateMachines.Movement
{
    /// <summary>
    /// Player State Machine which is an extension of base state machine class.
    /// Responsible for creating and storing instances of each state
    /// so they can be reused efficiently throughout the game.
    /// </summary>
    public class PlayerStateMachine : StateMachine.StateMachine
    {
        public PlayerController PlayerController { get; }
        public PlayerStateReusableData ReusableData { get; }

        public PlayerIdleState IdleState { get; }

        public PlayerWalkState WalkState { get; }
        public PlayerRunState RunState { get; }

        public PlayerLandState LandState { get; }

        public PlayerJumpState JumpState { get; }
        public PlayerFallState FallState { get; }

        public PlayerStateMachine(PlayerController playerController)
        {
            PlayerController = playerController;

            ReusableData = new PlayerStateReusableData();

            IdleState = new PlayerIdleState(this);

            WalkState = new PlayerWalkState(this);
            RunState = new PlayerRunState(this);

            LandState = new PlayerLandState(this);

            JumpState = new PlayerJumpState(this);
            FallState = new PlayerFallState(this);
        }
    }
}