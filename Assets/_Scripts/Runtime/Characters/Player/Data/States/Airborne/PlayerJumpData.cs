using UnityEngine;

namespace ProjectEmbersteel.Player.Data.States.Airborne
{
    /// <summary>
    /// Contains data related to player jump state, including forces and slope modifiers.
    /// </summary>
    [System.Serializable]
    public class PlayerJumpData
    {
        [field: SerializeField] public Vector3 StationaryForce { get; private set; }
        [field: SerializeField] public Vector3 WeakForce { get; private set; }
        [field: SerializeField] public Vector3 MediumForce { get; private set; }
        [field: SerializeField][field: Range(0f, 10f)] public float JumpCooldown { get; private set; } = 0.8f;
    }
}