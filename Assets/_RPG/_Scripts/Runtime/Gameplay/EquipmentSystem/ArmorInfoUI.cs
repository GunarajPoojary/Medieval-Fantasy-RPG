using AYellowpaper.SerializedCollections;
using RPG.Gameplay.Inventories;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Gameplay.EquipmentSystem
{
    public class ArmorInfoUI : EquipmentInfoUI
    {
        private ArmorSO[] _equippedArmors;
        private ArmorSO _selectedArmor;

        [SerializedDictionary("Armor Slot Type", "Armor Slot Content Transform")]
        [SerializeField] private SerializedDictionary<ArmorSlot, Transform> _armorSlotTypeToContentMap = new(6);
        private List<EquipmentSlot> _armorSlots;

        private void Awake()
        {
            _equippedArmors = EquipmentManager.Instance.EquippedArmorsSO
                .Where(a => a != null && !a.IsDefault)
                .ToArray();
            _selectedArmor = _equippedArmors.FirstOrDefault();

            EquipmentManager.Instance.OnArmorChanged += OnArmorChanged;
            EquipmentSlotManager.OnArmorSlotSelected += OnArmorSlotSelected;
        }

        protected override void OnEnable()
        {
            GetArmorSlots();
            if (_equippedArmors.Any())
            {
                _selectedArmor = _equippedArmors.First();
                UpdateUI(_selectedArmor);
                ToggleButtons(_selectedArmor);
            }
            else if (_armorSlots.Count > 0)
            {
                _armorSlots[Random.Range(0, _armorSlots.Count)].ShowItemInfo();
            }
        }

        private void GetArmorSlots() => _armorSlots = _armorSlotTypeToContentMap.Where(container => container.Value.childCount > 0)
                                                                        .SelectMany(container => container.Value.GetComponentsInChildren<EquipmentSlot>())
                                                                        .ToList();

        private void OnArmorChanged(ArmorSO newArmor, ArmorSO oldArmor)
        {
            if (newArmor != null && !newArmor.IsDefault)
                ToggleButtons(newArmor);
        }

        private void OnArmorSlotSelected(ArmorSO selectedArmor)
        {
            if (_equippedArmors.Contains(selectedArmor))
            {
                ToggleButtons(selectedArmor);
            }
            else
            {
                EnableButtons(setActiveEquipButton: true, setActiveUnequipButton: false, setActiveSwitchButton: false);
                UpdateUI(selectedArmor);
            }
        }

        protected override void ToggleButtons(EquipmentSO selectedEquipment)
        {
            var selectedArmor = (ArmorSO)selectedEquipment;
            if (_equippedArmors.Contains(selectedArmor))
            {
                EnableButtons(setActiveEquipButton: false, setActiveUnequipButton: true, setActiveSwitchButton: false);
            }
            else
            {
                EnableButtons(setActiveEquipButton: true, setActiveUnequipButton: false, setActiveSwitchButton: false);
            }
        }

        protected override void UpdateUI(EquipmentSO selectedEquipment)
        {
            var selectedArmor = (ArmorSO)selectedEquipment;
            if (selectedArmor == null) return;
            _selectedArmor = selectedArmor;

            _nameText.text = selectedArmor.name;
            _typeText.text = selectedArmor.ArmorType.ToString();
            _statsText.text = selectedArmor.EquipmentStats.ToString();
        }

        private void OnDisable()
        {
            EquipmentSlotManager.OnArmorSlotSelected -= OnArmorSlotSelected;
            EquipmentManager.Instance.OnArmorChanged -= OnArmorChanged;
        }

        public override void Equip() => _selectedArmor?.Use();

        public override void Unequip()
        {
            for (int i = 0; i < _equippedArmors.Length; i++)
            {
                if (_equippedArmors[i] == _selectedArmor)
                {
                    EquipmentManager.Instance.EquippedArmorsSO[i] = null;
                    break;
                }
            }
        }

        public override void Enhance()
        {
            // Enhancement logic
        }
    }
}
