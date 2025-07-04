using ProjectEmbersteel.Events.EventChannel;
using UnityEngine;

namespace ProjectEmbersteel.Loot
{
    [RequireComponent(typeof(CapsuleCollider))]
    public class ItemCollector : MonoBehaviour
    {
        [SerializeField] private LayerMask _pickables;
        [SerializeField] private IPickableEventChannelSO _onItemPickedEventChannel = default;

        private void OnTriggerEnter(Collider other)
        {
            if (((1 << other.gameObject.layer) & _pickables) == 0)
                return;

            if (other.TryGetComponent(out IPickable pickable))
                _onItemPickedEventChannel.RaiseEvent(pickable);
        }
    }
}