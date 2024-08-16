using RPG.ScriptableObjects.EventChannels;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.Inventories.UI
{
    public class ItemActionHandler : MonoBehaviour, IItemActionHandler
    {
        [SerializeField] private VoidReturnItemSOParameterEventChannelSO _removeItem;  // Event channel for removing an item

        public void SellItem(ItemSO item, GameObject slotGameObject)
        {
            _removeItem.RaiseEvent(item);
            Destroy(slotGameObject);
        }

        public void EnhanceItem(ItemSO item)
        {
            // Logic for enhancing the item
        }

        public void UseItem(ItemSO item)
        {
            // Logic for using the item
        }
    }
}