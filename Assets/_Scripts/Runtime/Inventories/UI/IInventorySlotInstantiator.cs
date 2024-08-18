using RPG.ScriptableObjects.Items;

namespace RPG.Inventories.UI
{
    public interface IInventorySlotInstantiator
    {
        void AddItemToUI(ItemSO item);
    }
}