using RPG.ScriptableObjects.Items;

namespace RPG.EquipmentSystem.UI
{
    // Interface for retrieving the currently equipped equipment
    public interface IEquippedEquipmentsGetter
    {
        EquipmentSO GetEquippedEquipment();
    }
}