using RPG.ScriptableObjects.Items;

namespace RPG.Inventories.UI
{
    /// <summary>
    /// Interface for setting Items in UI components.
    /// </summary>
    public interface IItemSetteable
    {
        void SetItem(ItemSO item);
    }
}