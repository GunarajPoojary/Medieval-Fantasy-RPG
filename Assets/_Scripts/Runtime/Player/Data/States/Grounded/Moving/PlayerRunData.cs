using UnityEngine;

namespace RPG
{
    [System.Serializable]
    public class PlayerRunData
    {
        [field: SerializeField][field: Range(1f, 2f)] public float SpeedModifier { get; private set; } = 1f;
        [field: SerializeField][field: Range(0f, 2f)] public float RunToWalkTime { get; private set; } = 0.5f;
    }
}