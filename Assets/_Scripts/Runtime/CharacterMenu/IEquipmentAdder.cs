namespace RPG
{
    /// <summary>
    /// Interface for adding equipment to the character menu.
    /// </summary>
    public interface IEquipmentAdder
    {
        void AddEquipment(EquipmentSO item);
    }
}