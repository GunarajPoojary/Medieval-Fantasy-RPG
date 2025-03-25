using UnityEngine;

namespace RPG
{
    [System.Serializable]
    public class PlayerCameraData
    {
        [field: SerializeField] public Transform CinemachineCameraTarget { get; private set; }
        [field: SerializeField] public float TopClamp { get; private set; } = 70.0f;
        [field: SerializeField] public float BottomClamp { get; private set; } = -30.0f;
    }
}