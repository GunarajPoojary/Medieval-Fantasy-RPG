using ProjectEmbersteel.Player.Data.States;
using ProjectEmbersteel.Player.StateMachines.Movement.States.Airborne;
using ProjectEmbersteel.Player.StateMachines.Movement.States.Grounded;
using ProjectEmbersteel.Player.StateMachines.Movement.States.Grounded.Landing;
using ProjectEmbersteel.Player.StateMachines.Movement.States.Grounded.Moving;

namespace ProjectEmbersteel.Player.StateMachines.Movement
{
    /// <summary>
    /// Factory and container for all player states.
    /// Responsible for creating and storing instances of each state
    /// so they can be reused efficiently throughout the game.
    /// </summary>
    public class PlayerStateFactory : StateMachine.StateMachine
    {
        public PlayerController PlayerController { get; }
        public PlayerStateReusableData ReusableData { get; }

        public PlayerIdleState IdleState { get; }

        public PlayerWalkState WalkState { get; }
        public PlayerRunState RunState { get; }

        public PlayerLightLandState LightLandState { get; }
        public PlayerRollState RollState { get; }
        public PlayerHardLandState HardLandState { get; }

        public PlayerJumpState JumpState { get; }
        public PlayerFallState FallState { get; }

        public PlayerStateFactory(PlayerController playerController)
        {
            PlayerController = playerController;
            
            ReusableData = new PlayerStateReusableData();

            IdleState = new PlayerIdleState(this);

            WalkState = new PlayerWalkState(this);
            RunState = new PlayerRunState(this);

            LightLandState = new PlayerLightLandState(this);
            RollState = new PlayerRollState(this);
            HardLandState = new PlayerHardLandState(this);

            JumpState = new PlayerJumpState(this);
            FallState = new PlayerFallState(this);
        }
    }
}