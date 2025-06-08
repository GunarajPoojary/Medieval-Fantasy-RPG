using RPG.Player.Data.Colliders;
using RPG.Utilities.Colliders;
using UnityEngine;

namespace RPG.Player.Utilities.Colliders
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