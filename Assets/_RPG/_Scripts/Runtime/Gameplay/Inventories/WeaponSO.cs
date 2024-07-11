using UnityEngine;

namespace RPG.Gameplay.Inventories
{
    /// <summary>
    /// Enumeration of different weapon types.
    /// </summary>
    public enum WeaponType
    {
        BowAndArrow,
        DualSwords,
        SwordAndShield,
        Staff,
        LongSword,
        Axe,
        Spear
    }

    /// <summary>
    /// Represents a specific type of equipment: a weapon.
    /// Inherits from EquipmentSO and adds additional properties specific to weapons.
    /// </summary>
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Items/Weapon", order = 1)]
    public class WeaponSO : EquipmentSO
    {
        public AnimatorOverrideController AnimatorOverride;
        public GameObject[] WeaponPrefabs;
        public WeaponType WeaponType;

        /// <summary>
        /// Validates the weapon item settings in the Unity editor.
        /// Sets the item type in Unity editor.
        /// </summary>

#if UNITY_EDITOR
        private void OnValidate() => Type = ItemType.Weapon;
#endif
    }
}
