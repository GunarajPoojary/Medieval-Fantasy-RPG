using RPG.ScriptableObjects.EventChannels;
using System.Collections;
using UnityEngine;

namespace RPG.World
{
    public class Collector : MonoBehaviour
    {
        [SerializeField] private float _sphereRadius = 5f;
        [SerializeField] private float _checkInterval = 0.5f;
        [SerializeField] private LayerMask _pickables;
        [Space]
        [SerializeField] private VoidReturnItemSOParameterEventChannelSO _addItem;  // Event channel to add picked up items to the inventory

        private WaitForSeconds _waitForSeconds;

        private void Awake() => _waitForSeconds = new WaitForSeconds(_checkInterval);

        private IEnumerator Start()
        {
            yield return _waitForSeconds;

            while (true)
            {
                PickUpItems();
                yield return _waitForSeconds;
            }
        }

        private void OnDisable() => StopAllCoroutines();

        private void PickUpItems()
        {
            var hitColliders = Physics.OverlapSphere(transform.position, _sphereRadius, _pickables);

            foreach (var collider in hitColliders)
            {
                if (collider.TryGetComponent(out IPickable pickable))
                {
                    _addItem.RaiseEvent(pickable.GetPickUpItem());
                    Destroy(collider.gameObject);
                }
            }
        }
    }
}