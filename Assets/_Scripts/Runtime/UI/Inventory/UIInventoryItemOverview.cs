using System;
using RPG.Events.EventChannel;
using RPG.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Inventory
{
    public class UIInventoryItemOverview : MonoBehaviour
    {
        [SerializeField] private GameObject _overviewPanel;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _statsText;
        [SerializeField] private Image _itemIcon;
        [SerializeField] private Button _useButton;
        [SerializeField] private Button _equipButton;
        [SerializeField] private Button _sellButton;
        [SerializeField] private Button[] _closeButton;
        [Header("Broadcasting")]
        [SerializeField] private ItemSOEventChannelSO _equipEquipmentEventChannel;
        private ItemSO _currentSelectedItem;

        private void OnEnable()
        {
            _useButton.onClick.AddListener(OnUseButtonClick);
            _sellButton.onClick.AddListener(OnSellButtonClick);
            _equipButton.onClick.AddListener(OnEquipButtonClick);

            foreach (Button button in _closeButton)
                button.onClick.AddListener(ToggleMenu);
        }

        private void OnDisable()
        {
            _useButton.onClick.RemoveListener(OnUseButtonClick);
            _sellButton.onClick.RemoveListener(OnSellButtonClick);
            _equipButton.onClick.RemoveListener(OnEquipButtonClick);

            foreach (Button button in _closeButton)
                button.onClick.RemoveListener(ToggleMenu);
        }

        public void DisplayItemOverview(ItemSO item)
        {
            _currentSelectedItem = item;

            switch (item.Type)
            {
                case ItemType.OneHandedSword:
                case ItemType.GreatSword:
                case ItemType.BowAndArrow:
                case ItemType.HeadArmor:
                case ItemType.ChestArmor:
                case ItemType.ArmArmor:
                case ItemType.BeltArmor:
                case ItemType.LegArmor:
                case ItemType.FeetArmor:
                    _useButton.gameObject.SetActive(false);
                    _equipButton.gameObject.SetActive(true);
                    _sellButton.gameObject.SetActive(true);
                    break;
                case ItemType.Edible:
                case ItemType.Potion:
                    _useButton.gameObject.SetActive(true);
                    _equipButton.gameObject.SetActive(false);
                    _sellButton.gameObject.SetActive(true);
                    break;
                case ItemType.Ore:
                case ItemType.SpecialItem:
                    // TODO: May implement Non-Usables
                    break;
            }

            _titleText.text = _currentSelectedItem.DisplayName;
            _descriptionText.text = _currentSelectedItem.ItemDescription;
            _itemIcon.sprite = _currentSelectedItem.Icon;

            ToggleMenu();
        }

        private void ToggleMenu()
        {
            if (_overviewPanel.activeSelf)
                _overviewPanel.SetActive(false);
            else
                _overviewPanel.SetActive(true);
        }

        private void OnSellButtonClick()
        {
            if (_currentSelectedItem != null)
                Debug.Log($"Sell {_currentSelectedItem.DisplayName}");
        }

        private void OnUseButtonClick()
        {
            if (_currentSelectedItem != null)
                Debug.Log($"Use {_currentSelectedItem.DisplayName}");
        }

        private void OnEquipButtonClick()
        {
            if (_currentSelectedItem != null)
                _equipEquipmentEventChannel.RaiseEvent(_currentSelectedItem);
        }
    }
}