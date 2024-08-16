using UnityEngine;

namespace RPG.Player.Data.States
{
    [System.Serializable]
    public class PlayerRotationData
    {
        [field: SerializeField] public Vector3 TargetRotationReachTime { get; private set; }
    }
}