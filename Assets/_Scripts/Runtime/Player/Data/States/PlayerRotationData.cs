using UnityEngine;

namespace RPG
{
    [System.Serializable]
    public class PlayerRotationData
    {
        [field: SerializeField] public Vector3 TargetRotationReachTime { get; private set; }
    }
}