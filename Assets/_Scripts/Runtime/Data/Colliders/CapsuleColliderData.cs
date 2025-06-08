using UnityEngine;

namespace RPG.Data.Colliders
{
    /// <summary>
    /// Encapsulates relevant data extracted from a CapsuleCollider,
    /// such as its reference, center position in local space, and vertical extents.
    /// Useful for collision checks, ground detection, and character physics.
    /// </summary>
    public class CapsuleColliderData
    {
        public CapsuleCollider Collider { get; private set; }
        public Vector3 ColliderCenterInLocalSpace { get; private set; }
        public Vector3 ColliderVerticalExtents { get; private set; }

        public CapsuleColliderData(CapsuleCollider collider)
        {
            Collider = collider;
            UpdateColliderData();
        }

        /// <summary>
        /// Updates the collider data fields based on the current state of the CapsuleCollider.
        /// Should be called if the collider is resized or repositioned.
        /// </summary>
        public void UpdateColliderData()
        {
            ColliderCenterInLocalSpace = Collider.center;
            ColliderVerticalExtents = new Vector3(0f, Collider.bounds.extents.y, 0f);
        }
    }
}