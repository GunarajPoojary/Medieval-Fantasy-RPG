namespace RPG
{
    // Interface for toggling and enabling/disabling UI buttons based on the selected equipment
    public interface IButtonsToggler
    {
        void ToggleButtons(EquipmentSO selectedEquipment);

        void EnableButtons(bool setActiveEquipButton, bool setActiveUnequipButton, bool setActiveEnhanceButton);
    }
}