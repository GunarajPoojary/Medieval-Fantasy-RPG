using UnityEngine;

namespace RPG.ScriptableObjects.Items
{
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

    [CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Items/Equipment/Weapon", order = 1)]
    public class WeaponSO : EquipmentSO
    {
        public WeaponType WeaponType;
        public AnimatorOverrideController GameplayAnimatorOverrideController;  // Animator override controller for gameplay scene
        public AnimatorOverrideController CharacterMenuAnimatorOverrideController;  // Animator override controller for character menu scene
        public GameObject[] WeaponPrefabs;

        private void OnValidate() => Type = ItemType.Weapon;
    }
}