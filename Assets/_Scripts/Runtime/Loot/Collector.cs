using System.Collections;
using UnityEngine;

namespace RPG
{
    public class Collector : MonoBehaviour
    {
        [SerializeField] private float _detectionRadius = 5f;
        [SerializeField] private float _checkInterval = 0.5f;
        [SerializeField] private LayerMask _pickables;
        [SerializeField] private ItemSOEventChannel _itemAdd;

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

        private void OnDrawGizmos() => Gizmos.DrawWireSphere(transform.position + Vector3.up, _detectionRadius);

        private void PickUpItems()
        {
            var hitColliders = Physics.OverlapSphere(transform.position + Vector3.up, _detectionRadius, _pickables);

            foreach (var collider in hitColliders)
            {
                if (collider.TryGetComponent(out IPickable pickable))
                {
                    _itemAdd.Invoke(pickable.GetPickUpItem());
                    Destroy(collider.gameObject);
                }
            }
        }
    }
}