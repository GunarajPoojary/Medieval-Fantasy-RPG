namespace RPG.Gameplay.Inventories
{
    /// <summary>
    /// Represents an abstract base class for all equippable items in the game.
    /// Inherits from ItemSO and includes additional functionality for equipment.
    /// </summary>
    public abstract class EquipmentSO : ItemSO
    {
        public Stats.BaseStats EquipmentStats;

        /// <summary>
        /// Equips the item by calling the Equip method on the EquipmentManager instance.
        /// </summary>
        public override void Use() => EquipmentSystem.EquipmentManager.Instance.Equip(this);
    }
}
