using TMPro;
using UnityEngine;

namespace RPG
{
    public abstract class EquipmentOverviewDisplayer : MonoBehaviour, IButtonsToggler, IEquipmentSlotsProvider, IEquippedEquipmentsProvider, IEquipmentActionHandler
    {
        [SerializeField] protected TextMeshProUGUI _nameText;
        [SerializeField] protected TextMeshProUGUI _itemDescription;
        [SerializeField] protected TextMeshProUGUI _statsText;
        [SerializeField] protected TextMeshProUGUI _typeText;

        [SerializeField] protected GameObject _equipButton;
        [SerializeField] protected GameObject _unequipButton;
        [SerializeField] protected GameObject _enhanceButton;

        protected virtual void OnEnable() => GetEquipmentSlots();

        #region IButtonsToggler Methods
        public abstract void ToggleButtons(EquipmentSO selectedEquipment);

        public void EnableButtons(bool setActiveEquipButton, bool setActiveUnequipButton, bool setActiveEnhanceButton)
        {
            _equipButton.SetActive(setActiveEquipButton);
            _unequipButton.SetActive(setActiveUnequipButton);
            _enhanceButton.SetActive(setActiveEnhanceButton);
        }
        #endregion

        #region IEquipmentActionHandler Methods
        public virtual void Equip() => EnableButtons(false, true, true);

        public virtual void Unequip() => EnableButtons(true, false, true);

        public abstract void Enhance();
        #endregion

        #region IEquipmentSlotsGetter Method
        // Abstract method to retrieve all equipment slots in the UI
        public abstract void GetEquipmentSlots();
        #endregion

        #region IEquippedEquipmentsGetter Method
        // Abstract method to retrieve the currently equipped equipment
        public abstract EquipmentSO GetEquippedEquipment();
        #endregion

        // Updates the overview UI with the selected equipment's details
        protected virtual void UpdateOverview<T>(T selectedEquipment) where T : EquipmentSO
        {
            if (selectedEquipment == null)
            {
                return;
            }

            _nameText.text = selectedEquipment.name;
            _itemDescription.text = selectedEquipment.Description;
            _statsText.text = selectedEquipment.EquipmentStats.ToString();
        }
    }
}