using UnityEngine;

namespace RPG.Player.Data.States.Airborne
{
    /// <summary>
    /// Contains data related to player fall state, such as fall speed limits and thresholds for hard landings.
    /// </summary>
    [System.Serializable]
    public class PlayerFallData
    {
        [field: Tooltip("Having higher numbers might not read collisions with shallow colliders correctly.")]
        [field: SerializeField][field: Range(0f, 10f)] public float FallSpeedLimit { get; private set; } = 10f;

        [field: SerializeField][field: Range(0f, 100f)] public float MinimumDistanceToBeConsideredHardFall { get; private set; } = 3f;
    }
}