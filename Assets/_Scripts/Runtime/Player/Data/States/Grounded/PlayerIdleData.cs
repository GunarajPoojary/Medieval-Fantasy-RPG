using System.Collections.Generic;
using UnityEngine;

namespace RPG.Player.Data.States
{
    [System.Serializable]
    public class PlayerIdleData
    {
        [field: SerializeField] public List<PlayerCameraRecenteringData> BackwardsCameraRecenteringData { get; private set; }
    }
}