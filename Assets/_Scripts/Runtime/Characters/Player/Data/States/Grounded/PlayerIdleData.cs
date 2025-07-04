using UnityEngine;

namespace ProjectEmbersteel.Player.Data.States.Grounded
{
    /// <summary>
    /// Contains data for player idle state, primarily speed adjustment.
    /// </summary>
    [System.Serializable]
    public class PlayerIdleData
    {
        [field: SerializeField] public float SpeedModifier { get; private set; } = 0.0f;
    }
}