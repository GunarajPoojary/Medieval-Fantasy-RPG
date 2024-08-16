using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.Inventories.UI
{
    /// <summary>
    /// Interface for handling Item actions like selling, enhancing, or using items.
    /// </summary>
    public interface IItemActionHandler
    {
        void SellItem(ItemSO item, GameObject slotGameObject);
        void EnhanceItem(ItemSO item);
        void UseItem(ItemSO item);
    }
}