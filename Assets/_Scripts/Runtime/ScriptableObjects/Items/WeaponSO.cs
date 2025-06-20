using UnityEngine;

namespace RPG.Item
{
    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Custom/Items/Weapon", order = 1)]
    public class WeaponSO : EquipmentSO
    {
        [SerializeField] private AnimatorOverrideController _animatorOverrideController;
        public AnimatorOverrideController AnimatorOverrideController => _animatorOverrideController;
    }
}