namespace GunarajCode.ScriptableObjects
{
    /// <summary>
    /// Represents an abstract base class for all equippable items in the game.
    /// Inherits from ItemObject and includes additional functionality for equipment.
    /// </summary>
    public abstract class EquipmentObject : ItemObject
    {
        // The stats associated with this piece of equipment
        public Stats EquipmentStats;

        /// <summary>
        /// Equips the item by calling the Equip method on the EquipmentManager instance.
        /// Removes the item from the inventory afterwards.
        /// </summary>
        public override void Use()
        {
            EquipmentManager.Instance.Equip(this);
            RemoveFromInventory();
        }
    }
}