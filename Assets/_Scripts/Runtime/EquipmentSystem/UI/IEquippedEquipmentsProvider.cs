namespace RPG
{
    // Interface for retrieving the currently equipped equipment
    public interface IEquippedEquipmentsProvider
    {
        EquipmentSO GetEquippedEquipment();
    }
}