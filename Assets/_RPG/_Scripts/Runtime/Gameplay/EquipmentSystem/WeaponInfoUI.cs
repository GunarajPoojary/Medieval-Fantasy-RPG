using RPG.Gameplay.Inventories;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Gameplay.EquipmentSystem
{
    public class WeaponInfoUI : EquipmentInfoUI
    {
        private WeaponSO _equippedWeapon;
        private WeaponSO _selectedWeapon;
        [SerializeField] private Transform _weaponsContainerUI;
        private List<EquipmentSlot> _weaponSlots;

        private void Awake()
        {
            _equippedWeapon = EquipmentManager.Instance.EquippedWeaponSO;
            _selectedWeapon = _equippedWeapon;
            EquipmentManager.Instance.OnWeaponChanged += OnWeaponChanged;
            EquipmentSlotManager.OnWeaponSlotSelected += OnWeaponSlotSelected;
        }

        private void OnWeaponChanged(WeaponSO newWeapon, WeaponSO oldWeapon) => ToggleButtons(newWeapon);

        private void OnWeaponSlotSelected(WeaponSO selectedWeapon)
        {
            if (selectedWeapon == EquipmentManager.Instance.EquippedWeaponSO)
            {
                ToggleButtons(selectedWeapon);
            }
            else if (selectedWeapon != EquipmentManager.Instance.EquippedWeaponSO && EquipmentManager.Instance.EquippedWeaponSO != null)
            {
                EnableButtons(setActiveEquipButton: false, setActiveUnequipButton: false, setActiveSwitchButton: true);
                UpdateUI(selectedWeapon);
            }
            else if (selectedWeapon != EquipmentManager.Instance.EquippedWeaponSO && EquipmentManager.Instance.EquippedWeaponSO == null)
            {
                EnableButtons(setActiveEquipButton: true, setActiveUnequipButton: false, setActiveSwitchButton: false);
                UpdateUI(selectedWeapon);
            }
        }

        protected override void OnEnable()
        {
            GetWeaponSlots();
            if (_equippedWeapon != null)
            {
                UpdateUI(_equippedWeapon);
                ToggleButtons(_equippedWeapon);
            }
            else
            {
                _weaponSlots[Random.Range(0, _weaponSlots.Count)].ShowItemInfo();
            }
        }

        private void GetWeaponSlots()
        {
            _weaponSlots = new();

            _weaponSlots = _weaponsContainerUI.GetComponentsInChildren<EquipmentSlot>()
                                              .Where(slot => _weaponsContainerUI.childCount > 0)
                                              .ToList();
        }

        private void OnDestroy()
        {
            EquipmentSlotManager.OnWeaponSlotSelected -= OnWeaponSlotSelected;
            EquipmentManager.Instance.OnWeaponChanged -= OnWeaponChanged;
        }

        protected override void UpdateUI(EquipmentSO selectedEquipment)
        {
            var selectedWeapon = (WeaponSO)selectedEquipment;
            if (selectedWeapon == null) return;

            _selectedWeapon = selectedWeapon;

            _nameText.text = selectedWeapon.name;
            _typeText.text = selectedWeapon.WeaponType.ToString();
            _statsText.text = selectedWeapon.EquipmentStats.ToString();
        }

        protected override void ToggleButtons(EquipmentSO selectedEquipment)
        {
            var selectedWeapon = (WeaponSO)selectedEquipment;
            if (selectedWeapon == EquipmentManager.Instance.EquippedWeaponSO)
            {
                EnableButtons(setActiveEquipButton: false, setActiveUnequipButton: true, setActiveSwitchButton: false);
            }
            else if (selectedWeapon != EquipmentManager.Instance.EquippedWeaponSO && EquipmentManager.Instance.EquippedWeaponSO != null)
            {
                EnableButtons(setActiveEquipButton: false, setActiveUnequipButton: false, setActiveSwitchButton: true);
            }
            else if (selectedWeapon != EquipmentManager.Instance.EquippedWeaponSO && EquipmentManager.Instance.EquippedWeaponSO == null)
            {
                EnableButtons(setActiveEquipButton: true, setActiveUnequipButton: false, setActiveSwitchButton: false);
            }
        }

        public override void Equip() => _selectedWeapon?.Use();

        public override void Unequip() => EquipmentManager.Instance.EquippedWeaponSO = null;

        public override void Enhance()
        {
            // Enhancement logic
        }
    }
}
