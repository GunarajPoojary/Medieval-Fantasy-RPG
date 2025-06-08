using UnityEngine;

namespace RPG.Player.Data.States.Grounded.Moving
{
    /// <summary>
    /// Contains data related to player run state, including speed and transition timing.
    /// </summary>
    [System.Serializable]
    public class PlayerRunData
    {
        [field: SerializeField][field: Range(1f, 2f)] public float SpeedModifier { get; private set; } = 1f;
        [field: SerializeField][field: Range(0f, 2f)] public float RunToWalkTime { get; private set; } = 0.5f;
    }
}