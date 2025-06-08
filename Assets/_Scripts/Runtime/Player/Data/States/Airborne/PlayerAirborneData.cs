using UnityEngine;

namespace RPG.Player.Data.States.Airborne
{
    /// <summary>
    /// Contains data related to the player's airborne state, including jump and fall state.
    /// </summary>
    [System.Serializable]
    public class PlayerAirborneData
    {
        [field: SerializeField] public PlayerJumpData JumpData { get; private set; }
        [field: SerializeField] public PlayerFallData FallData { get; private set; }
    }
}