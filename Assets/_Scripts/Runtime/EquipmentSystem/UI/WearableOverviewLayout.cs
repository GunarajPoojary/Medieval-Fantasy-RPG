using AYellowpaper.SerializedCollections;
using RPG.ScriptableObjects.EventChannels;
using RPG.ScriptableObjects.Items;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.EquipmentSystem.UI
{
    public class WearableOverviewLayout : EquipmentOverviewLayout
    {
        // Dictionary to map wearable slot types to their respective UI content transforms
        [SerializedDictionary("Wearable Slot Type", "Wearable Slot Content Transform")]
        [SerializeField] private SerializedDictionary<WearableSlot, Transform> _wearableSlotTypeToContentMap;
        [Space]
        // Event channel to handle when a wearable slot is selected
        [SerializeField] private VoidReturnWearableSOParameterEventChannelSO _wearableSlotSelectedEventChannel;
        [Space]
        // Event channel to handle when a wearable is equipped or unequipped
        [SerializeField] private VoidReturnDoubleWearableSOParameterEventChannelSO _wearableChangedEventChannelSO;
        [Space]
        // Event channel to get the currently equipped wearables
        [SerializeField] private WearableSOArrayReturnNonParameterEventChannelSO _equippedWearableEventChannelSO;
        [Space]
        // Event channel to equip a wearable
        [SerializeField] private VoidReturnWearableSOParameterEventChannelSO _equippWearableEventChannelSO;
        [Space]
        // Event channel to unequip a wearable
        [SerializeField] private WearableSOReturnIntParameterEventChannelSO _unequipWearableEventChannelSO;

        private WearableSO _selectedWearable;
        private WearableSO[] _equippedWearables; // Array to store all currently equipped wearables

        // List of wearable slot UI elements
        private List<EquipmentSlotUI> _wearableSlots;

        protected override void OnEnable()
        {
            base.OnEnable();

            _wearableSlotSelectedEventChannel.OnEventRaised += HandleWearableSlotSelected;

            _equippedWearables = new WearableSO[System.Enum.GetNames(typeof(WearableSlot)).Length];

            _wearableChangedEventChannelSO.OnEventRaised += HandleWearableChanged;

            _selectedWearable = (WearableSO)GetEquippedEquipment();

            ShowRandomWearableOverview();
        }

        private void OnDisable()
        {
            _wearableChangedEventChannelSO.OnEventRaised -= HandleWearableChanged;

            _wearableSlotSelectedEventChannel.OnEventRaised -= HandleWearableSlotSelected;
        }

        #region IEquippedEquipmentsGetter Method
        // Retrieves the currently equipped wearable and stores it in _equippedWearables
        public override EquipmentSO GetEquippedEquipment()
        {
            WearableSO[] equippedWearables = _equippedWearableEventChannelSO.RaiseEvent();

            for (int i = 0; i < _equippedWearables.Length; i++)
            {
                _equippedWearables[i] = equippedWearables[i];
            }

            return _equippedWearables[0]; // Assume the first one is the main equipped item
        }
        #endregion

        #region IEquipmentSlotsGetter Method
        // Retrieves all wearable slots in the UI
        public override void GetEquipmentSlots()
        {
            _wearableSlots = new List<EquipmentSlotUI>();

            foreach (var container in _wearableSlotTypeToContentMap.Values)
            {
                if (container.childCount > 0)
                {
                    foreach (var displayer in container.GetComponentsInChildren<EquipmentSlotUI>())
                    {
                        _wearableSlots.Add(displayer);
                    }
                }
            }
        }
        #endregion

        #region IButtonsToggler Methods
        // Toggles the state of equip, unequip, and enhance buttons based on the selected wearable
        public override void ToggleButtons(EquipmentSO selectedEquipment)
        {
            var selectedWearable = (WearableSO)selectedEquipment;

            if (_equippedWearables.Contains(selectedWearable))
            {
                EnableButtons(false, true, true);
            }
            else
            {
                EnableButtons(true, false, true);
            }
        }
        #endregion

        #region IEquipmentActionHandler methods
        // Handles equipping the selected wearable and updates the equipped wearables array
        public override void Equip()
        {
            base.Equip();

            _equippWearableEventChannelSO.RaiseEvent(_selectedWearable);
            int index = (int)_selectedWearable.EquipSlot;
            _equippedWearables[index] = _selectedWearable;
        }

        // Handles unequipping the selected wearable and updates the equipped wearables array
        public override void Unequip()
        {
            base.Unequip();

            int index = (int)_selectedWearable.EquipSlot;
            _unequipWearableEventChannelSO.RaiseEvent(index);
            _equippedWearables[index] = null;
        }

        public override void Enhance()
        {
            // Enhancement logic (to be implemented)
        }
        #endregion

        // Displays a random wearable overview or the currently equipped one if available
        private void ShowRandomWearableOverview()
        {
            if (_equippedWearables.Length > 0 && _equippedWearables[0] != null)
            {
                _selectedWearable = _equippedWearables[0];
                UpdateOverview(_selectedWearable);
                ToggleButtons(_selectedWearable);
            }
            else if (_wearableSlots.Count > 0)
            {
                _wearableSlots[Random.Range(0, _wearableSlots.Count)].DisplayItemOverview();
            }
        }

        // Handles the event when a wearable is changed (equipped/unequipped)
        private void HandleWearableChanged(WearableSO newWearable, WearableSO oldWearable)
        {
            _equippedWearables[(int)newWearable.EquipSlot] = newWearable;
            ToggleButtons(newWearable);
        }

        // Handles the event when a wearable slot is selected
        private void HandleWearableSlotSelected(WearableSO selectedWearable)
        {
            _selectedWearable = selectedWearable;
            UpdateOverview(selectedWearable);
            ToggleButtons(selectedWearable);
        }

        // Updates the UI overview to display information about the selected wearable
        protected override void UpdateOverview<T>(T selectedEquipment)
        {
            base.UpdateOverview<T>(selectedEquipment);

            if (selectedEquipment is WearableSO selectedWearable)
            {
                _typeText.text = selectedWearable.WearableType.ToString();
            }
        }
    }
}