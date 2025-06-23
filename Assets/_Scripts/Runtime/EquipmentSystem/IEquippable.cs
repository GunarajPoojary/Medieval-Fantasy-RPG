using ProjectEmbersteel.StatSystem;

namespace ProjectEmbersteel.EquipmentSystem
{
    public interface IEquippable
    {
        bool IsEquipped{ get; }
        void Equip(IStatModifiable statModifiable);
        void Unequip(IStatModifiable statModifiable);
    }
}
