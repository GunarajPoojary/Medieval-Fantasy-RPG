using UnityEngine.Events;
using UnityEngine;

namespace RPG
{
    /// <summary>
    /// This class is used for Events that have a ItemSO argument.
    /// Example: An event to notify Inventory when item in the world picked up
    /// </summary>
    [CreateAssetMenu(menuName = "Game/Events/ItemSO Event Channel")]
    public class ItemSOEventChannelSO : DescriptionBaseSO
    {
        public event UnityAction<ItemSO> OnEventRaised;

        public void RaiseEvent(ItemSO item) => OnEventRaised?.Invoke(item);
    }
}