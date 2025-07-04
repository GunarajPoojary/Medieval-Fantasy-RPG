using UnityEngine;

namespace ProjectEmbersteel.Player.Data.Layers
{
    /// <summary>
    /// Holds data related to player layer interactions, specifically ground detection.
    /// </summary>
    [System.Serializable]
    public class PlayerLayerData
    {
        [field: SerializeField] public LayerMask GroundLayer { get; private set; }
    }
}