using UnityEngine;

namespace ProjectEmbersteel.Player.Data.States.Airborne
{
    /// <summary>
    /// Contains data related to the player's airborne state, including jump and fall state.
    /// </summary>
    [System.Serializable]
    public class PlayerAirborneData
    {
        [field: SerializeField] public PlayerJumpData JumpData { get; private set; }
        [field: SerializeField] public float GravityMultiplier { get; private set; } = 1f;
    }
}