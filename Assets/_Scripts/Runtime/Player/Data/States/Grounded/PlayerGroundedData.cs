using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [System.Serializable]
    public class PlayerGroundedData
    {
        [field: SerializeField][field: Range(0f, 25f)] public float BaseSpeed { get; private set; } = 5f;
        [field: SerializeField][field: Range(0f, 5f)] public float GroundToFallRayDistance { get; private set; } = 1f;
        [field: SerializeField] public AnimationCurve SlopeSpeedAngles { get; private set; }
        [field: SerializeField] public PlayerRotationData BaseRotationData { get; private set; }
        [field: SerializeField] public PlayerIdleData IdleData { get; private set; }
        [field: SerializeField] public PlayerWalkData WalkData { get; private set; }
        [field: SerializeField] public PlayerRunData RunData { get; private set; }
        [field: SerializeField] public PlayerRollData RollData { get; private set; }
    }
}