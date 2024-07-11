using UnityEngine;

namespace RPG.Gameplay.World
{
    /// <summary>
    /// Handles the logic for the player to collect items within a specified radius.
    /// Uses a spherical overlap check to detect items that can be picked up.
    /// </summary>
    public class Collector : MonoBehaviour
    {
        [SerializeField] private float _sphereRadius;
        [SerializeField] private LayerMask _pickables;

        private void Update() => PickUpItems();

        private void PickUpItems()
        {
            var hitColliders = Physics.OverlapSphere(transform.position, _sphereRadius, _pickables);
            foreach (var collider in hitColliders)
            {
                if (collider.TryGetComponent(out IPickable pickable))
                    pickable.PickUp();
            }
        }
    }
}