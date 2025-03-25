using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    [System.Serializable]
    public class PlayerIdleData
    {
        [field: SerializeField] public float SpeedModifier { get; private set; } = 0.0f;
    }
}