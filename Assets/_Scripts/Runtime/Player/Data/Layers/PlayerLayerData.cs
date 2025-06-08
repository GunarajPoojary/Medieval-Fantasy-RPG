using UnityEngine;

namespace RPG.Player.Data.Layers
{
    /// <summary>
    /// Holds data related to player layer interactions, specifically ground detection.
    /// </summary>
    [System.Serializable]
    public class PlayerLayerData
    {
        [field: SerializeField] public LayerMask GroundLayer { get; private set; }

        public bool ContainsLayer(LayerMask layerMask, int layer) => (1 << layer & layerMask) != 0;

        public bool IsGroundLayer(int layer) => ContainsLayer(GroundLayer, layer);
    }
}