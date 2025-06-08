using UnityEngine;

namespace RPG
{
    public enum WeaponType
    {
        Default,
        GreatSword
    }

    [CreateAssetMenu(fileName = "NewWeapon", menuName = "Game/Inventory/Items/Equipment/Weapon", order = 1)]
    public class WeaponSO : EquipmentSO
    {
        [SerializeField] private WeaponType _weaponType;
        [SerializeField] private AnimatorOverrideController _gameplayAnimatorOverrideController;
        [SerializeField] private AnimatorOverrideController _characterMenuAnimatorOverrideController;
        [SerializeField] private GameObject[] _weaponPrefabs;

        public WeaponType WeaponType => _weaponType;
        public AnimatorOverrideController GameplayAnimator => _gameplayAnimatorOverrideController;
        public AnimatorOverrideController CharacterMenuAnimator => _characterMenuAnimatorOverrideController;
        public GameObject[] WeaponPrefabs => _weaponPrefabs;

        protected override void OnValidate()
        {
            _type = ItemType.Weapon;

            base.OnValidate();
        }

    }
}