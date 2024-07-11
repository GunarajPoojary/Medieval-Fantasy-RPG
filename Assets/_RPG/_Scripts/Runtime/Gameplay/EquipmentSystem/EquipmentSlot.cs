using RPG.Gameplay.Inventories;

namespace RPG.Gameplay.EquipmentSystem
{
    public class EquipmentSlot : ItemSlotUI
    {
        private EquipmentSO _equipment;
        //private bool _isEquipped;

        /// <summary>
        /// Sets the item associated with this slot and updates the icon.
        /// </summary>
        /// <param name="equipment">The item to be associated with this slot.</param>
        public override void SetItem(ItemSO equipment)
        {
            //if (_equipment == EquipmentManager.Instance.EquippedWeaponSO)
            //    _isEquipped = true;
            //else
            //    _isEquipped = false;

            _equipment = equipment as EquipmentSO;

            _icon.sprite = _equipment.Icon;
        }

        public override void ShowItemInfo()
        {
            if (_equipment is WeaponSO weapon)
                EquipmentSlotManager.ShowWeaponSelected(weapon);
            else if (_equipment is ArmorSO armor)
                EquipmentSlotManager.ShowArmorSelected(armor);
        }
    }
}
