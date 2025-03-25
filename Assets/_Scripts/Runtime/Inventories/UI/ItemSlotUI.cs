using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG
{
    /// <summary>
    /// Abstract base class for item slot UI components, handling icon updates and interaction events.
    /// </summary>
    public abstract class ItemSlotUI : MonoBehaviour
    {
        [SerializeField] protected Image _icon;
        [SerializeField] protected InventorySlotEventChannel inventorySlotSelectedEvent;
        public ItemSO Item { get; private set; }

        public virtual void Initialize(ItemSO item)
        {
            Item = item;
            _icon.sprite = Item.Icon;
        }
    }
}