using RPG.Inventories.UI;
using RPG.ScriptableObjects.EventChannels;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.EquipmentSystem
{
    public class EquipmentSlotUI : ItemSlotUI, IOverviewDisplayer
    {
        [SerializeField] private VoidReturnWeaponSOParameterEventChannelSO _weaponSlotSelectedEventChannel; // Event channel for weapon slot selection
        [Space]
        [SerializeField] private VoidReturnWearableSOParameterEventChannelSO _wearableSlotSelectedEventChannel; // Event channel for wearable slot selection

        private EquipmentSO _equipment;

        // Sets the item associated with this slot and updates the equipment reference
        public override void SetItem(ItemSO item)
        {
            _equipment = (EquipmentSO)item;
            base.SetItem(item);
        }

        // Displays the overview for the item in this slot
        public override void DisplayItemOverview()
        {
            if (_equipment is WeaponSO weapon)
            {
                _weaponSlotSelectedEventChannel.RaiseEvent(weapon);
            }
            else if (_equipment is WearableSO wearable)
            {
                _wearableSlotSelectedEventChannel.RaiseEvent(wearable);
            }
        }

        protected override void UpdateIcon() => _icon.sprite = _equipment?.Icon;
    }
}