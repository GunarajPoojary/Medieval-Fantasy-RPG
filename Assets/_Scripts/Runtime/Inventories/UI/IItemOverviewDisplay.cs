using RPG.ScriptableObjects.Items;

namespace RPG.Inventories.UI
{
    /// <summary>
    /// Interface for displaying Item details in the UI.
    /// </summary>
    public interface IItemOverviewDisplay
    {
        void DisplayItemOverview(ItemSO item);
        void HideOverviewLayout();
    }
}