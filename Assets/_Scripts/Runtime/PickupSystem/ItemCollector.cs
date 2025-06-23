using ProjectEmbersteel.Events.EventChannel;
using UnityEngine;

namespace ProjectEmbersteel.Loot
{
    public class ItemCollector : MonoBehaviour
    {
        [SerializeField] private LayerMask _pickables;
        [SerializeField] private IPickableEventChannelSO _onItemPickedEventChannel = default;

        private void OnTriggerEnter(Collider collider)
        {
            if (((1 << collider.gameObject.layer) & _pickables) != 0)
            {
                if (collider.TryGetComponent(out IPickable pickable))
                {
                    if (pickable != null)
                        _onItemPickedEventChannel.RaiseEvent(pickable);
                }
            }
        }
    }
}