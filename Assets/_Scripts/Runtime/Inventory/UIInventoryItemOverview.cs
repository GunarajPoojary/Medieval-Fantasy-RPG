using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG
{
    public class UIInventoryItemOverview : MonoBehaviour
    {
        [SerializeField] private GameObject _defaultItemOverviewPanel;
        [SerializeField] private GameObject _containerPanel;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Image _itemIcon;
        [SerializeField] private Button _useButton;
        [SerializeField] private Button _sellButton;
        [SerializeField] private TMP_Text _statsText;
        private ItemSO _currentSelectedItem;

        private void OnEnable()
        {
            _useButton.onClick.AddListener(OnUseButtonClick);
            _sellButton.onClick.AddListener(OnSellButtonClick);
        }

        private void OnDisable()
        {
            _useButton.onClick.RemoveListener(OnUseButtonClick);
            _sellButton.onClick.RemoveListener(OnSellButtonClick);
        }

        public void SetSelectedItem(ItemSO item)
        {
            _currentSelectedItem = item;
            _titleText.text = _currentSelectedItem.DisplayName;
            _descriptionText.text = _currentSelectedItem.ItemDescription;
            _itemIcon.sprite = _currentSelectedItem.Icon;

            _containerPanel.SetActive(true);
            _defaultItemOverviewPanel.SetActive(false);
        }

        public void ResetSelection()
        {
            _currentSelectedItem = null;
            _containerPanel.SetActive(false);
            _defaultItemOverviewPanel.SetActive(true);
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
    }
}