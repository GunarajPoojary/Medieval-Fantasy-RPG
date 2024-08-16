using RPG.ScriptableObjects.Items;

namespace RPG.Inventories.UI
{
    /// <summary>
    /// Interface for setting Items in UI components.
    /// </summary>
    public interface IItemSetter
    {
        void SetItem(ItemSO item);
    }
}