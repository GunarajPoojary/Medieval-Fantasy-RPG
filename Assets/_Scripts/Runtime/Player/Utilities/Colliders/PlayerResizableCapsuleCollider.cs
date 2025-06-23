using ProjectEmbersteel.Player.Data.Colliders;
using ProjectEmbersteel.Utilities.Colliders;
using UnityEngine;

namespace ProjectEmbersteel.Player.Utilities.Colliders
{
    public class PlayerResizableCapsuleCollider : ResizableCapsuleCollider
    {
        [field: SerializeField, Tooltip("Data holding configuration and runtime info for the player's ground check trigger collider.")]
        
        public PlayerTriggerColliderData TriggerColliderData { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            TriggerColliderData.Initialize();
        }
    }
}