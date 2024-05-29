using GunarajCode.ScriptableObjects;
using UnityEngine;

namespace GunarajCode.Inventories
{
    /// <summary>
    /// Represents an item in the game world that can be picked up by the player.
    /// Implements the IPickable interface to allow interaction with the player's inventory.
    /// </summary>
    public class ItemPickUp : MonoBehaviour, IPickable
    {
        [SerializeField] private ItemObject _item;

        public void PickUp()
        {
            Inventory.Instance.Add(_item);
            Destroy(gameObject);
        }
    }
}