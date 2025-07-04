using UnityEngine;

namespace ProjectEmbersteel.Player.Data.States.Grounded
{
    /// <summary>
    /// Contains data related to the player's grounded state, including move data and idle data.
    /// </summary>
    [System.Serializable]
    public class PlayerGroundedData
    {
        [field: SerializeField] public PlayerIdleData IdleData { get; private set; }
        [field: SerializeField] public PlayerMoveData MoveData { get; private set; }
    }
}