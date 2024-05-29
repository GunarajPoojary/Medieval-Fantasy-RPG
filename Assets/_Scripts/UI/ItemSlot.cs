using GunarajCode.ScriptableObjects;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GunarajCode
{
    /// <summary>
    /// Represents a slot for holding items in the UI.
    /// </summary>
    public class ItemSlot : MonoBehaviour
    {
        protected ItemObject _item;
        [SerializeField] protected Image _icon;

        [SerializeField] protected Button _button;

        public event Action<ItemObject, GameObject> OnButtonPressed;

        private void Awake() => _button.onClick.AddListener(ShowItemInfo);

        /// <summary>
        /// Sets the item associated with this slot and updates the icon.
        /// </summary>
        /// <param name="item">The item to be associated with this slot.</param>
        public void SetItem(ItemObject item)
        {
            _item = item;
            _icon.sprite = _item.Icon;
        }

        public void ShowItemInfo()
        {
            OnButtonPressed?.Invoke(_item, gameObject);
        }

        /// <summary>
        /// Uses the item associated with this slot and destroys the slot object.
        /// </summary>
        public void UseItem()
        {
            _item.Use();
            Destroy(gameObject);
        }
    }
}
