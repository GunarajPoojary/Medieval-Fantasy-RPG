using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    /// <summary>
    /// Displays detailed information about a selected Item in the inventory.
    /// </summary>
    public class InventoryItemOverviewUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _itemDescription;
        [SerializeField] private GameObject _overviewContainer;
        [SerializeField] private Image _image;
        [SerializeField] private ItemSOEventChannel _inventoryItemRemoveChannel;

        private ItemSO _item;

        private InventorySlotUI _selectedSlotObject;

        private void OnEnable()
        {
            HideOverviewLayout();
        }

        public void DisplayItemOverview(InventorySlotUI item)
        {
            _selectedSlotObject = item;
            _item = item.Item;
            _nameText.text = _item.Name;
            _itemDescription.text = _item.Description;
            _image.sprite = _item.Icon;
            _overviewContainer.SetActive(true);
        }

        /// <summary>
        /// Hides the item overview UI.
        /// </summary>
        private void HideOverviewLayout() => _overviewContainer.SetActive(false);

        public void SellItem()
        {
            _inventoryItemRemoveChannel.Invoke(_item);
            Destroy(_selectedSlotObject.gameObject);
            HideOverviewLayout();
        }

        public void EnhanceItem()
        {
            // Logic for enhancing the item
        }

        public void UseItem()
        {
            // Logic for using the item
        }
    }
}