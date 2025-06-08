using UnityEngine;

namespace RPG
{
    public enum WearableSlot
    {
        Head,
        Torso,
        Back,
        Hands,
        Legs,
        Feet
    }

    [CreateAssetMenu(fileName = "NewWearable", menuName = "Game/Inventory/Items/Equipment/Wearable", order = 2)]
    public class WearableSO : EquipmentSO
    {
        [SerializeField] private WearableSlot _equipSlot;

        public WearableSlot EquipSlot => _equipSlot;
        //public SkinnedMeshRenderer SkinnedMesh;

        protected override void OnValidate()
        {
            _type = ItemType.Wearable;

            base.OnValidate();
        }

    }
}