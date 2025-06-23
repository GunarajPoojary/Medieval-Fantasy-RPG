using ProjectEmbersteel.Data.Colliders;
using UnityEngine;

namespace ProjectEmbersteel.Utilities.Colliders
{
    /// <summary>
    /// Base class responsible for dynamically adjusting the 
    /// size and shape of a CapsuleCollider attached to the GameObject.
    /// </summary>
    [RequireComponent(typeof(CapsuleCollider))]
    public class ResizableCapsuleCollider : MonoBehaviour
    {
        private CapsuleCollider _collider;

        public CapsuleColliderData CapsuleColliderData { get; private set; }

        [Tooltip("Default configuration values for the capsule collider (height, radius, center).")]
        [field: SerializeField] public DefaultColliderData DefaultColliderData { get; private set; }

        [Tooltip("Slope data that affects the capsule collider height based on step height percentage.")]
        [field: SerializeField] public SlopeData SlopeData { get; private set; }

        private void Awake() => Resize();

        private void OnValidate() => Resize();

        /// <summary>
        /// Resize the collider according to default and slope data
        /// </summary>
        public void Resize()
        {
            if (_collider == null) _collider = GetComponent<CapsuleCollider>();

            Initialize();

            // Apply the new dimensions to the capsule collider
            UpdateCapsuleColliderDimensions();
        }

        public void Initialize()
        {
            if (CapsuleColliderData != null) return;

            CapsuleColliderData = new CapsuleColliderData(_collider);

            OnInitialize();
        }

        // Updates the capsule collider's radius, height, center, and stores new data
        public void UpdateCapsuleColliderDimensions()
        {
            SetCapsuleColliderRadius(DefaultColliderData.Radius);

            SetCapsuleColliderHeight(DefaultColliderData.Height * (1f - SlopeData.StepHeightPercentage));

            SetCapsuleColliderCenter();

            RecalculateColliderRadius();

            CapsuleColliderData.UpdateColliderData();
        }

        public void SetCapsuleColliderRadius(float radius) => CapsuleColliderData.Collider.radius = radius;

        public void SetCapsuleColliderHeight(float height) => CapsuleColliderData.Collider.height = height;

        public void SetCapsuleColliderCenter()
        {
            // Calculate how much the height changed from the default
            float colliderHeightDifference = DefaultColliderData.Height - CapsuleColliderData.Collider.height;

            // Shift the center up by half the height difference
            Vector3 newColliderCenter = new(0f, DefaultColliderData.CenterY + (colliderHeightDifference / 2f), 0f);

            // Apply the new center
            CapsuleColliderData.Collider.center = newColliderCenter;
        }

        // Adjust radius if it's greater than half the height (invalid in Unity physics)
        public void RecalculateColliderRadius()
        {
            float halfColliderHeight = CapsuleColliderData.Collider.height / 2f;

            if (halfColliderHeight >= CapsuleColliderData.Collider.radius) return;

            SetCapsuleColliderRadius(halfColliderHeight);
        }

        protected virtual void OnInitialize() { }
    }
}