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

    public enum WearableType
    {
        HelmetOrMask,
        ShirtOrBodyArmor,
        GlovesOrGauntlets,
        Cape,
        PantOrLegArmor,
        Boots
    }

    [CreateAssetMenu(fileName = "New Wearable", menuName = "Inventory/Items/Equipment/Wearable", order = 2)]
    public class WearableSO : EquipmentSO
    {
        public WearableSlot EquipSlot;
        public WearableType WearableType;
        //public SkinnedMeshRenderer SkinnedMesh;

        private void OnValidate() => Type = ItemType.Wearable;
    }
}