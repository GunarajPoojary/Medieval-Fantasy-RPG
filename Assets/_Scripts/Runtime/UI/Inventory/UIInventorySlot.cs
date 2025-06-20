using System.Text;
using RPG.Inventory;
using RPG.Item;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RPG.UI.Inventory
{
    public class UIInventorySlot : UISelectableButton<ItemSO>, IDeselectHandler
    {
        [Header("UI References")]
        [SerializeField] private Image _itemIcon;
        [SerializeField] private TextMeshProUGUI _stackCountText;
        [SerializeField] private Image _selectedImage;
        [SerializeField] private Image _hoverImage;
        [SerializeField] private AudioClip _selectedSound;
        [SerializeField] private AudioClip _hoverSound;

        private static IDeselectHandler _currentSelected;
        private AudioSource _audioSource;
        private InventoryItem _item;
        private readonly StringBuilder _stringBuilder = new(16);

        public void Initialize(InventoryItem inventoryItem, Transform parentTransform, AudioSource audioSource)
        {
            _item = inventoryItem;
            _audioSource = audioSource;

            transform.SetParent(parentTransform);

            SetIcon();
            UpdateStackCount();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (_audioSource != null)
                _audioSource.PlayOneShot(_hoverSound);

            _hoverImage.gameObject.SetActive(true);
        }

        public override void OnPointerExit(PointerEventData eventData) => _hoverImage.gameObject.SetActive(false);

        public void OnDeselect(BaseEventData eventData) => _selectedImage.gameObject.SetActive(false);

        public override void OnPointerClick(PointerEventData eventData)
        {
            if (_currentSelected != null && _currentSelected != this as IDeselectHandler)
                _currentSelected.OnDeselect(eventData);

            if (_audioSource != null)
                _audioSource.PlayOneShot(_selectedSound);

            _selectedImage.gameObject.SetActive(true);

            base.OnPointerClick(eventData);

            _currentSelected = this;
        }

        public void UpdateStackCount()
        {
            _stringBuilder.Clear();
            _stringBuilder.Append("x ");
            _stringBuilder.Append(_item?.StackCount ?? 0); // Fallback if null
            _stackCountText.text = _stringBuilder.ToString(); // One allocation here
        }

        protected override ItemSO GetValue() => _item.Item;

        private void SetIcon() => _itemIcon.sprite = _item?.Item.Icon;
    }
}