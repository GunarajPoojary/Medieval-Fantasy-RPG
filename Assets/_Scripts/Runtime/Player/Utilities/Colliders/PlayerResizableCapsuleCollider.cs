using RPG.Player.Data;
using RPG.Utils;
using UnityEngine;

namespace RPG.Player.Utils
{
    public class PlayerResizableCapsuleCollider : ResizableCapsuleCollider
    {
        [field: SerializeField] public PlayerTriggerColliderData TriggerColliderData { get; private set; }

        protected override void OnInitialize()
        {
            base.OnInitialize();

            TriggerColliderData.Initialize();
        }
    }
}