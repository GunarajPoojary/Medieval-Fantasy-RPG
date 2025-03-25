using UnityEngine;

namespace RPG
{
    public class EquippedWeaponOverviewDisplay : EquippedEquipmentOverviewDisplay
    {
        //[SerializeField] private WeaponSOReturnNonParameterEventChannelSO _equippedWeaponEventChannelSO; // Event channel to get equipped weapon
        //[Space]
        //[SerializeField] private VoidReturnDoubleWeaponSOParameterEventChannelSO _weaponChangedEventChannelSO; // Event channel for weapon changes

        private void OnEnable()
        {
            //_weaponChangedEventChannelSO.OnEventRaised += OnWeaponChanged;

            // Display the equipped weapon when enabled
            //UpdateOverview(_equippedWeaponEventChannelSO.RaiseEvent());
        }

        private void OnDisable()
        {
            //_weaponChangedEventChannelSO.OnEventRaised -= OnWeaponChanged;
        }

        // Updates the overview when the weapon changes
        private void OnWeaponChanged(WeaponSO newWeapon, WeaponSO oldWeapon) => UpdateOverview(newWeapon);

        // Returns the type text specific to the weapon equipment
        protected override string GetTypeText(EquipmentSO equipment)
        {
            WeaponSO weapon = (WeaponSO)equipment;
            return weapon.WeaponType.ToString();
        }
    }
}