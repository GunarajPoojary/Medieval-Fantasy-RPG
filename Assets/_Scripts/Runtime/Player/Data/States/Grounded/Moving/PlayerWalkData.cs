using UnityEngine;

namespace ProjectEmbersteel.Player.Data.States.Grounded.Moving
{
    /// <summary>
    /// Contains data for player walk state, including speed adjustment.
    /// </summary>
    [System.Serializable]
    public class PlayerWalkData
    {
        [field: SerializeField][field: Range(0f, 1f)] public float SpeedModifier { get; private set; } = 0.225f;
    }
}