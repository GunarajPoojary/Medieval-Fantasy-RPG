using RPG.ScriptableObjects.Items;

namespace RPG.Inventories.UI
{
    /// <summary>
    /// Interface for displaying Item details in the UI.
    /// </summary>
    public interface IItemOverviewDisplayer
    {
        void DisplayItemOverview(ItemSO item);
        void HideOverviewLayout();
    }
}