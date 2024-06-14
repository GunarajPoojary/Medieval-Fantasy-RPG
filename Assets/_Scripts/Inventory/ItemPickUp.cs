using GunarajCode.ScriptableObjects;
using UnityEngine;

namespace GunarajCode.Inventories
{
    /// <summary>
    /// Represents an item in the game world that can be picked up by the player.
    /// Implements the IPickable interface to allow interaction with the player's inventory.
    /// </summary>
    public class ItemPickUp : MonoBehaviour, IPickable, IDataPersistence
    {
        [SerializeField] private ItemObject _item;
        [SerializeField] private string _id;
        private bool _collected = false;

        [ContextMenu("Generate guid for id")]
        private void GenerateGuid()
        {
            _id = System.Guid.NewGuid().ToString();
        }

        public void PickUp()
        {
            Inventory.Instance.Add(_item);
            _collected = true;
            Destroy(gameObject);
        }

        public void LoadData(GameData data)
        {
            data.ItemCollected.TryGetValue(_id, out _collected);
            if (_collected)
            {
                Destroy(gameObject);
                //gameObject.SetActive(false);
            }
        }

        public void SaveData(GameData data)
        {
            if (data.ItemCollected.ContainsKey(_id))
            {
                data.ItemCollected.Remove(_id);
            }
            data.ItemCollected.Add(_id, _collected);
        }
    }
}