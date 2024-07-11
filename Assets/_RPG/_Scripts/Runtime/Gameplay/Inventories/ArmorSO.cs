using UnityEngine;

namespace RPG.Gameplay.Inventories
{
    /// <summary>
    /// Armor slots for equippable armor items.
    /// </summary>
    public enum ArmorSlot
    {
        Head,
        Torso,
        Back,
        Hands,
        Legs,
        Feet
    }

    /// <summary>
    /// Different types of armor.
    /// </summary>
    public enum ArmorType
    {
        None,
        Helmet,
        Shirt,
        Gloves,
        Cape,
        Pant,
        Boots
    }

    /// <summary>
    /// Represents a specific type of equipment: armor.
    /// Inherits from EquipmentSO and adds additional properties specific to armor.
    /// </summary>
    [CreateAssetMenu(fileName = "New Armor", menuName = "Inventory/Items/Armor", order = 2)]
    public class ArmorSO : EquipmentSO
    {
        public bool IsDefault;
        public SkinnedMeshRenderer SkinnedMesh;
        public ArmorSlot ArmorEquipSlot;
        public ArmorType ArmorType;

        /// <summary>
        /// Validates the armor settings in the Unity editor.
        /// If the armor is set as default, resets certain fields to default values.
        /// </summary>

#if UNITY_EDITOR
        private void OnValidate()
        {
            Type = ItemType.Armor;
            if (IsDefault)
            {
                Description = default;
                Icon = null;
                EquipmentStats = null;
            }
        }
#endif
    }
}
