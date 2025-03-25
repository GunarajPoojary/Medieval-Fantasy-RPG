using UnityEngine;

namespace RPG
{
    /// <summary>
    /// Handles the character's Stats when equipment is equipped or unequipped in Character Menu
    /// </summary>
    public class EquipmentStatsHandler : MonoBehaviour
    {
        //[SerializeField] private VoidReturnDoubleWeaponSOParameterEventChannelSO _weaponChangedEventChannelSO;  // Event channel for weapon changes
        [Space]
        //[SerializeField] private VoidReturnDoubleWearableSOParameterEventChannelSO _wearableChangedEventChannelSO;  // Event channel for wearable changes
        [Space]
        [SerializeField] private RuntimeStats _runtimeStats;

        private void OnEnable()
        {
            //_weaponChangedEventChannelSO.OnEventRaised += OnWeaponChanged;
            //_wearableChangedEventChannelSO.OnEventRaised += OnWearableChanged;
        }

        private void OnDisable()
        {
            //_weaponChangedEventChannelSO.OnEventRaised -= OnWeaponChanged;
            //_wearableChangedEventChannelSO.OnEventRaised -= OnWearableChanged;
        }

        private void OnWeaponChanged(WeaponSO newWeapon, WeaponSO oldWeapon)
        {
            ChangeCharacterStats(oldWeapon, -1);
            ChangeCharacterStats(newWeapon, 1);
        }

        private void OnWearableChanged(WearableSO newArmor, WearableSO oldArmor)
        {
            ChangeCharacterStats(oldArmor, -1);
            ChangeCharacterStats(newArmor, 1);
        }

        // Adds or removes stats from equipment.
        private void ChangeCharacterStats(EquipmentSO equipment, int operation)
        {
            if (equipment == null)
            {
                return;
            }

            foreach (var stat in equipment.EquipmentStats.GetAllStats())
            {
                _runtimeStats.ChangeOverallStatValue(stat.Key, stat.Value * operation);
                _runtimeStats.ChangeCurrentStatValue(stat.Key, stat.Value * operation);
            }
        }
    }
}