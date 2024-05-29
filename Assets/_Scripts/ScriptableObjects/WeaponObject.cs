using UnityEngine;

namespace GunarajCode.ScriptableObjects
{
    /// <summary>
    /// Represents a specific type of equipment: a weapon.
    /// Inherits from EquipmentObject and adds additional properties specific to weapons.
    /// </summary>
    [CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Items/Weapon", order = 1)]
    public class WeaponObject : EquipmentObject
    {
        public AnimatorOverrideController AnimatorOverride;
        public GameObject[] WeaponPrefabs;
    }
}