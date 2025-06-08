using RPG.Player.Data.States.Grounded.Landing;
using RPG.Player.Data.States.Grounded.Moving;
using UnityEngine;

namespace RPG.Player.Data.States.Grounded
{
    /// <summary>
    /// Contains data related to the player's grounded state, including movement speeds, rotation data, and slope behavior.
    /// </summary>
    [System.Serializable]
    public class PlayerGroundedData
    {
        [field: SerializeField][field: Range(0f, 25f)] public float BaseSpeed { get; private set; } = 5f;
        [field: SerializeField][field: Range(0f, 5f)] public float GroundToFallRayDistance { get; private set; } = 1f;
        [field: SerializeField][field: Range(0f, 2f)] public float JumpDelay { get; private set; } = 0.7f;
        [field: SerializeField] public AnimationCurve SlopeSpeedAngles { get; private set; }
        [field: SerializeField] public PlayerRotationData BaseRotationData { get; private set; }
        [field: SerializeField] public PlayerIdleData IdleData { get; private set; }
        [field: SerializeField] public PlayerWalkData WalkData { get; private set; }
        [field: SerializeField] public PlayerRunData RunData { get; private set; }
        [field: SerializeField] public PlayerRollData RollData { get; private set; }
    }
}