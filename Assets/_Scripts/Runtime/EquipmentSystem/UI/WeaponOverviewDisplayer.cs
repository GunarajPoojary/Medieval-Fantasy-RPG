using RPG.ScriptableObjects;
using RPG.ScriptableObjects.EventChannels;
using RPG.ScriptableObjects.Items;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.EquipmentSystem.UI
{
    public class WeaponOverviewDisplayer : EquipmentOverviewDisplayer
    {
        [SerializeField] private Transform _weaponsContainerUI;
        [Space]
        [SerializeField] private VoidReturnWeaponSOParameterEventChannelSO _weaponSlotSelectedEventChannel; // Event channel for weapon slot selection
        [Space]
        [SerializeField] private WeaponSOReturnNonParameterEventChannelSO _equippedWeaponEventChannelSO; // Event channel to get equipped weapon
        [Space]
        [SerializeField] private VoidReturnDoubleWeaponSOParameterEventChannelSO _weaponChangedEventChannelSO; // Event channel to handle weapon change
        [Space]
        [SerializeField] private VoidReturnWeaponSOParameterEventChannelSO _equipWeaponSOEventChannelSO; // Event channel to equip weapon
        [Space]
        [SerializeField] private WeaponSOReturnNonParameterEventChannelSO _unequipWeaponSOEventChannelSO; // Event channel to unequip weapon

        private WeaponSO _equippedWeapon;
        private WeaponSO _selectedWeapon;

        private List<IOverviewDisplayer> _weaponSlots; // List of weapon slots in the UI

        protected override void OnEnable()
        {
            base.OnEnable();

            _weaponSlotSelectedEventChannel.OnEventRaised += HandleWeaponSelected;

            _weaponChangedEventChannelSO.OnEventRaised += HandleWeaponChanged;

            _selectedWeapon = (WeaponSO)GetEquippedEquipment();

            DisplayInitialOverview();
        }

        private void OnDisable()
        {
            _weaponSlotSelectedEventChannel.OnEventRaised -= HandleWeaponSelected;

            _weaponChangedEventChannelSO.OnEventRaised -= HandleWeaponChanged;
        }

        #region IEquippedEquipmentsGetter Method
        // Retrieves the currently equipped weapon
        public override EquipmentSO GetEquippedEquipment()
        {
            _equippedWeapon = _equippedWeaponEventChannelSO.RaiseEvent();

            return _equippedWeapon;
        }
        #endregion

        #region IEquipmentSlotsGetter Method
        // Retrieves all weapon slots in the UI
        public override void GetEquipmentSlots()
        {
            _weaponSlots = new List<IOverviewDisplayer>();
            foreach (Transform child in _weaponsContainerUI)
            {
                IOverviewDisplayer displayer = child.GetComponent<EquipmentSlotUI>();
                if (displayer != null)
                {
                    _weaponSlots.Add(displayer);
                }
            }
        }
        #endregion

        #region IButtonsToggler Methods
        // Toggles the state of equip, unequip, and enhance buttons based on the selected weapon
        public override void ToggleButtons(EquipmentSO selectedEquipment)
        {
            var selectedWeapon = (WeaponSO)selectedEquipment;

            if (selectedWeapon == _equippedWeapon)
            {
                EnableButtons(false, true, true);
            }
            else
            {
                EnableButtons(true, false, true);
            }
        }
        #endregion

        #region IEquipmentActionHandler Methods
        // Handles equipping the selected weapon
        public override void Equip()
        {
            _equipWeaponSOEventChannelSO.RaiseEvent(_selectedWeapon);

            base.Equip();
        }

        // Handles unequipping the selected weapon
        public override void Unequip()
        {
            _unequipWeaponSOEventChannelSO.RaiseEvent();

            base.Unequip();
        }

        public override void Enhance()
        {
            // Enhancement logic (to be implemented)
        }
        #endregion

        // Handles the event when the equipped weapon changes
        private void HandleWeaponChanged(WeaponSO newWeapon, WeaponSO oldWeapon)
        {
            _equippedWeapon = newWeapon;
            ToggleButtons(newWeapon);
        }

        // Displays the initial overview for the equipped weapon or a random weapon
        private void DisplayInitialOverview()
        {
            if (_equippedWeapon != null)
            {
                UpdateOverview(_equippedWeapon);
                ToggleButtons(_equippedWeapon);
            }
            else
            {
                _weaponSlots[Random.Range(0, _weaponSlots.Count)].DisplayItemOverview();
            }
        }

        // Handles the event when a weapon slot is selected
        private void HandleWeaponSelected(WeaponSO selectedWeapon)
        {
            _selectedWeapon = selectedWeapon;
            UpdateOverview(selectedWeapon);
            ToggleButtons(_selectedWeapon);
        }

        // Updates the UI overview to display information about the selected weapon
        protected override void UpdateOverview<T>(T selectedEquipment)
        {
            base.UpdateOverview(selectedEquipment);

            if (selectedEquipment is WeaponSO selectedWeapon)
            {
                _typeText.text = selectedWeapon.WeaponType.ToString();
            }
        }
    }
}