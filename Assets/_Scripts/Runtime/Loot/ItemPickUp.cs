using RPG.Core.SaveLoad;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.World
{
    public class ItemPickUp : MonoBehaviour, IPickable, IIdentifiable, ISaveable
    {
        [Tooltip("Use context menu to generate ID")]
        [SerializeField] private string _id;
        [field: SerializeField] public ItemSO Item { get; private set; }

        private bool _collected = false;

        public string Id => _id;

        #region IIdentifiable Methods
        [ContextMenu("Generate guid for id")]
        public void GenerateGuid() => _id = System.Guid.NewGuid().ToString();
        #endregion

        #region IPickable Methods
        public ItemSO GetPickUpItem()
        {
            if (Item != null)
            {
                _collected = true;
                return Item;
            }
            else
            {
                Debug.LogError("The Item that you are trying to get is null");
                return null;
            }
        }

        #endregion

        #region ISaveable Methods
        public void LoadData(GameData data)
        {
            if (data.ItemCollected.TryGetValue(_id, out _collected) && _collected)
            {
                Destroy(gameObject);
            }
        }

        public void SaveData(GameData data) => data.ItemCollected[_id] = _collected;
        #endregion
    }
}