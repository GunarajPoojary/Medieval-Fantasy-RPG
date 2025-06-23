using UnityEngine.Events;
using UnityEngine;
using ProjectEmbersteel.Loot;

namespace ProjectEmbersteel.Events.EventChannel
{
    /// <summary>
    /// This class is used for Events that have a IPickable argument.
    /// Example: An event to notify Inventory when item in the world picked up
    /// </summary>
    [CreateAssetMenu(menuName = "Custom/Events/IPickable Event Channel")]
    public class IPickableEventChannelSO : DescriptionBaseSO
    {
        public event UnityAction<IPickable> OnEventRaised;

        public void RaiseEvent(IPickable pickable) => OnEventRaised?.Invoke(pickable);
    }
}