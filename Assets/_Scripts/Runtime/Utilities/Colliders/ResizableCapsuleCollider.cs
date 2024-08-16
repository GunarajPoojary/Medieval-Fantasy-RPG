using RPG.Data.Colliders;
using UnityEngine;

namespace RPG.Utils
{
    /// <summary>
    /// Manages the resizing of a capsule collider based on specified data.
    /// </summary>
    public class ResizableCapsuleCollider : MonoBehaviour
    {
        /// <summary>
        /// Data used for configuring the capsule collider.
        /// </summary>
        public CapsuleColliderData CapsuleColliderData { get; private set; }

        [field: SerializeField]
        /// <summary>
        /// Default data for the capsule collider, including radius and height.
        /// </summary>
        public DefaultColliderData DefaultColliderData { get; private set; }

        [field: SerializeField]
        /// <summary>
        /// Data related to slope adjustments, including step height percentage.
        /// </summary>
        public SlopeData SlopeData { get; private set; }

        private void Awake() => Resize();

        private void OnValidate() => Resize();

        /// <summary>
        /// Initializes the capsule collider data and recalculates its dimensions.
        /// </summary>
        public void Resize()
        {
            Initialize(gameObject);

            CalculateCapsuleColliderDimensions();
        }

        /// <summary>
        /// Initializes the capsule collider data if it has not already been initialized.
        /// </summary>
        /// <param name="gameObject">The game object to initialize the capsule collider data for.</param>
        public void Initialize(GameObject gameObject)
        {
            if (CapsuleColliderData != null)
            {
                return;
            }

            CapsuleColliderData = new CapsuleColliderData();

            CapsuleColliderData.Initialize(gameObject);

            OnInitialize();
        }

        /// <summary>
        /// Calculates the dimensions of the capsule collider based on default and slope data.
        /// </summary>
        public void CalculateCapsuleColliderDimensions()
        {
            SetCapsuleColliderRadius(DefaultColliderData.Radius);

            SetCapsuleColliderHeight(DefaultColliderData.Height * (1f - SlopeData.StepHeightPercentage));

            RecalculateCapsuleColliderCenter();

            RecalculateColliderRadius();

            CapsuleColliderData.UpdateColliderData();
        }

        /// <summary>
        /// Sets the radius of the capsule collider.
        /// </summary>
        /// <param name="radius">The radius to set.</param>
        public void SetCapsuleColliderRadius(float radius)
        {
            CapsuleColliderData.Collider.radius = radius;
        }

        /// <summary>
        /// Sets the height of the capsule collider.
        /// </summary>
        /// <param name="height">The height to set.</param>
        public void SetCapsuleColliderHeight(float height)
        {
            CapsuleColliderData.Collider.height = height;
        }

        /// <summary>
        /// Recalculates the center of the capsule collider based on its height and default center data.
        /// </summary>
        public void RecalculateCapsuleColliderCenter()
        {
            float colliderHeightDifference = DefaultColliderData.Height - CapsuleColliderData.Collider.height;

            Vector3 newColliderCenter = new Vector3(0f, DefaultColliderData.CenterY + (colliderHeightDifference / 2f), 0f);

            CapsuleColliderData.Collider.center = newColliderCenter;
        }

        /// <summary>
        /// Recalculates the radius of the capsule collider to ensure it is not smaller than half of its height.
        /// </summary>
        public void RecalculateColliderRadius()
        {
            float halfColliderHeight = CapsuleColliderData.Collider.height / 2f;

            if (halfColliderHeight >= CapsuleColliderData.Collider.radius)
            {
                return;
            }

            SetCapsuleColliderRadius(halfColliderHeight);
        }

        /// <summary>
        /// Virtual method to be overridden in derived classes if additional initialization logic is needed.
        /// </summary>
        protected virtual void OnInitialize()
        {
        }
    }
}