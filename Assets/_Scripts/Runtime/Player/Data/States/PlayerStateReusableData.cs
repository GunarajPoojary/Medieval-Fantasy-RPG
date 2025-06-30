using UnityEngine;

namespace ProjectEmbersteel.Player.Data.States
{
    /// <summary>
    /// Stores runtime reusable data for the player's movement.
    /// </summary>
    public class PlayerStateReusableData
    {
        private float _currentVerticalVelocity;
        private Vector3 _currentHorizontalVelocity;

        public Vector2 MovementInput { get; set; }
        public float MovementSpeedModifier { get; set; } = 1f;

        public bool ShouldRun { get; set; }

        public ref float CurrentVerticalVelocity => ref _currentVerticalVelocity;
        public ref Vector3 CurrentHorizontalVelocity => ref _currentHorizontalVelocity;

        public Vector3 CurrentJumpForce { get; set; }
    }
}