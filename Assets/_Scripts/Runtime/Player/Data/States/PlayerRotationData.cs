using UnityEngine;

namespace RPG.Player.Data.States
{
    /// <summary>
    /// Contains data for how quickly the player character reaches a target rotation.
    /// </summary>
    [System.Serializable]
    public class PlayerRotationData
    {
        [field: SerializeField] public Vector3 TargetRotationReachTime { get; private set; }
    }
}