using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Inventory
{
    public class UIInventorySlot : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TextMeshProUGUI _stackCountText;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Button _slotButton;

        private InventoryItem _currentItem;

        public event Action<ItemSO> ItemClicked;

        public ItemSO ItemSO => _currentItem.Item;

        private void OnEnable() => _slotButton.onClick.AddListener(OnSlotSelected);

        private void OnDisable() => _slotButton.onClick.RemoveListener(OnSlotSelected);

        public void Initialize(InventoryItem inventoryItem, Transform parentTransform)
        {
            _currentItem = inventoryItem;
            transform.SetParent(parentTransform);
            SetIcon();
            UpdateStackCount();
        }

        public void UpdateStackCount() => _stackCountText.text = _currentItem?.StackCount.ToString();

        private void OnSlotSelected() => ItemClicked?.Invoke(_currentItem.Item);

        private void SetIcon() => _itemIcon.sprite = _currentItem?.Item.Icon;
    }
}