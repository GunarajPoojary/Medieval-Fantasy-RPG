using RPG.Gameplay.Inventories;
using TMPro;
using UnityEngine;

namespace RPG.Gameplay.EquipmentSystem
{
    public abstract class EquipmentInfoUI : MonoBehaviour
    {
        [SerializeField] protected TextMeshProUGUI _nameText;
        [SerializeField] protected TextMeshProUGUI _statsText;
        [SerializeField] protected TextMeshProUGUI _typeText;

        [SerializeField] protected GameObject _equipButton;
        [SerializeField] protected GameObject _unequipButton;
        [SerializeField] protected GameObject _switchButton;

        protected abstract void OnEnable();

        protected abstract void UpdateUI(EquipmentSO selectedEquipment);

        protected abstract void ToggleButtons(EquipmentSO selectedEquipment);

        protected void EnableButtons(bool setActiveEquipButton, bool setActiveUnequipButton, bool setActiveSwitchButton)
        {
            _equipButton.SetActive(setActiveEquipButton);
            _unequipButton.SetActive(setActiveUnequipButton);
            _switchButton.SetActive(setActiveSwitchButton);
        }

        public abstract void Equip();

        public abstract void Unequip();

        public abstract void Enhance();
    }
}
