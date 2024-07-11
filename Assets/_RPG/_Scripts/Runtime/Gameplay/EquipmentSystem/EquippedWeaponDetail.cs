using RPG.Gameplay.Inventories;

namespace RPG.Gameplay.EquipmentSystem
{
    /// <summary>
    /// Class for displaying equipped weapon details in the UI.
    /// </summary>
    public class EquippedWeaponDetail : EquippedEquipmentDetail
    {
        private WeaponSO equippedWeapon;

        private void Awake()
        {
            EquipmentManager.Instance.OnWeaponChanged += OnWeaponChanged;
            equippedWeapon = EquipmentManager.Instance.EquippedWeaponSO;
            UpdateDetailUI(equippedWeapon);
        }

        private void OnWeaponChanged(WeaponSO newWeapon, WeaponSO oldWeapon) => UpdateDetailUI(newWeapon);

        private void OnDestroy() => EquipmentManager.Instance.OnWeaponChanged -= OnWeaponChanged;

        protected override string GetTypeText(EquipmentSO equipment)
        {
            var weapon = (WeaponSO)equipment;
            return weapon.WeaponType.ToString();
        }
    }
}
