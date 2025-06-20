using RPG.Item;
using UnityEngine;

namespace RPG.Loot
{
    /// <summary>
    /// This class represents a pickable Item in the world
    /// </summary>
    public class ItemPickUp : MonoBehaviour, IPickable
    {
        [field: SerializeField] public ItemSO Item { get; private set; }

        #region IPickable Methods
        public ItemSO PickUpItem() => Item;

        public void SetGameObject(bool collected) => gameObject.SetActive(collected);
        #endregion
    }
}