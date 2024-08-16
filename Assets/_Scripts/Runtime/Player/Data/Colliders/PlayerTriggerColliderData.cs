using UnityEngine;

namespace RPG.Player.Data
{
    [System.Serializable]
    public class PlayerTriggerColliderData
    {
        [field: SerializeField] public BoxCollider GroundCheckCollider { get; private set; }

        public Vector3 GroundCheckColliderVerticalExtents { get; private set; }

        public void Initialize() => GroundCheckColliderVerticalExtents = GroundCheckCollider.bounds.extents;
    }
}