using UnityEngine;

namespace RPG
{
    [System.Serializable]
    public class PlayerLayerData
    {
        [field: SerializeField] public LayerMask GroundLayer { get; private set; }

        public bool ContainsLayer(LayerMask layerMask, int layer) => (1 << layer & layerMask) != 0;

        public bool IsGroundLayer(int layer) => ContainsLayer(GroundLayer, layer);
    }
}