using UnityEngine;

namespace GunarajCode.ScriptableObjects
{
    /// <summary>
    /// Armor slots for equippable armor items.
    /// </summary>
    public enum ArmorSlot { Head, Torso, Cape, Hands, Legs, Feet }

    /// <summary>
    /// Represents a specific type of equipment: armor.
    /// Inherits from EquipmentObject and adds additional properties specific to armor.
    /// </summary>
    [CreateAssetMenu(fileName = "New Armor", menuName = "Inventory/Items/Armor", order = 2)]
    public class ArmorObject : EquipmentObject
    {
        public bool IsDefault;
        public SkinnedMeshRenderer SkinnedMesh;
        public ArmorSlot ArmorEquipSlot;

#if UNITY_EDITOR
        void OnValidate()
        {
            // If the armor is set as default, reset certain fields to default values
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