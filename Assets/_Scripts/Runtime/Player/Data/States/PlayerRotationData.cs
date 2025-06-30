using UnityEngine;

namespace ProjectEmbersteel.Player.Data.States
{
    /// <summary>
    /// Contains data for how quickly the player character reaches a target rotation.
    /// </summary>
    [System.Serializable]
    public class PlayerRotationData
    {
        [field: SerializeField] public float RotationDamping { get; private set; } = 0.5f;
    }
}