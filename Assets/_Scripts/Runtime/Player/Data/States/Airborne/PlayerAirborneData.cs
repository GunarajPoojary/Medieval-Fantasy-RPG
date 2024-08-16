using UnityEngine;

namespace RPG.Player.Data.States
{
    [System.Serializable]
    public class PlayerAirborneData
    {
        [field: SerializeField] public PlayerJumpData JumpData { get; private set; }
        [field: SerializeField] public PlayerFallData FallData { get; private set; }
    }
}