using ProjectEmbersteel.Player.Data.States.Grounded.Moving;
using UnityEngine;

namespace ProjectEmbersteel.Player.Data.States.Grounded
{
    /// <summary>
    /// Contains data related to the player's move state, including movement speeds, run state, walk state and rotation data
    /// </summary>
    [System.Serializable]
    public class PlayerMoveData
    {
        [field: SerializeField][field: Range(0f, 25f)] public float BaseSpeed { get; private set; } = 2f;
        [field: SerializeField] public PlayerRotationData BaseRotationData { get; private set; }
        [field: SerializeField] public PlayerWalkData WalkData { get; private set; }
        [field: SerializeField] public PlayerRunData RunData { get; private set; }
    }
}