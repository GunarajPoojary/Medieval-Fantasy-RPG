using UnityEngine;

namespace RPG.Loot
{
    public class ItemCollector : MonoBehaviour
    {
        [SerializeField] private LayerMask _pickables;
        [SerializeField] private ItemSOEventChannelSO _onItemPickedEventChannel = default;

        private void OnTriggerEnter(Collider collider)
        {
            if (((1 << collider.gameObject.layer) & _pickables) != 0)
            {
                if (collider.TryGetComponent(out IPickable pickable))
                {
                    ItemSO item = pickable.PickUpItem();

                    if (item != null)
                    {
                        PickableItemManager.Instance.OnTryPickingUpItem(pickable, item);
                        _onItemPickedEventChannel.RaiseEvent(item);
                    }
                }
            }
        }
    }
}