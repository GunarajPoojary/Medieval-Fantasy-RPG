using UnityEngine;

namespace ProjectEmbersteel.Player.Data.States.Grounded.Moving
{
    /// <summary>
    /// Contains data related to player run state, including speed.
    /// </summary>
    [System.Serializable]
    public class PlayerRunData
    {
        [field: SerializeField][field: Range(1f, 6f)] public float SpeedModifier { get; private set; } = 5.335f;
    }
}