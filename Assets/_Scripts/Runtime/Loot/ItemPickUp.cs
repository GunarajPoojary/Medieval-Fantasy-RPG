using RPG.Core.SaveLoad;
using RPG.ScriptableObjects.Items;
using UnityEngine;

namespace RPG.World
{
    public class ItemPickUp : MonoBehaviour, IPickable, ISaveable
    {
        [Tooltip("Use context menu to generate ID")]
        [field: SerializeField] public string Id { get; private set; }

        [field: SerializeField] public ItemSO Item { get; private set; }

        private bool _collected = false;

        [ContextMenu("Generate guid for id")]
        public void GenerateGuid() => Id = System.Guid.NewGuid().ToString();

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
            if (data.ItemCollected.TryGetValue(Id, out _collected) && _collected)
            {
                Destroy(gameObject);
            }
        }

        public void SaveData(GameData data) => data.ItemCollected[Id] = _collected;
        #endregion
    }
}