using UnityEngine;

namespace ProjectEmbersteel.Player.Data.States.Airborne
{
    /// <summary>
    /// Contains data related to player jump state, including forces and slope modifiers.
    /// </summary>
    [System.Serializable]
    public class PlayerJumpData
    {
        [field: SerializeField][field: Range(0f, 5f)] public float JumpToGroundRayDistance { get; private set; } = 2f;
        [field: SerializeField] public AnimationCurve JumpForceModifierOnSlopeUpwards { get; private set; }
        [field: SerializeField] public AnimationCurve JumpForceModifierOnSlopeDownwards { get; private set; }
        [field: SerializeField] public PlayerRotationData RotationData { get; private set; }
        [field: SerializeField] public Vector3 StationaryForce { get; private set; }
        [field: SerializeField] public Vector3 WeakForce { get; private set; }
        [field: SerializeField] public Vector3 MediumForce { get; private set; }
        [field: SerializeField][field: Range(0f, 10f)] public float DecelerationForce { get; private set; } = 1.5f;
    }
}