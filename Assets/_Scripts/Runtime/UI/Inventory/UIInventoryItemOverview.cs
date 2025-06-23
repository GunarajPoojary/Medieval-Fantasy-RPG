using ProjectEmbersteel.EquipmentSystem;
using ProjectEmbersteel.Events.EventChannel;
using ProjectEmbersteel.Item;
using ProjectEmbersteel.StatSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectEmbersteel.UI.Inventory
{
    [System.Serializable]
    public class StatOverviewUI
    {
        public GameObject gameObject;
        public StatType statType;
        public TMP_Text statValueText;
    }
    public class UIInventoryItemOverview : MonoBehaviour
    {
        [SerializeField] private GameObject _overviewPanel;
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private TMP_Text _primaryStatTypeText;
        [SerializeField] private TMP_Text _primaryStatValueText;
        [SerializeField] private Image _itemIcon;
        [SerializeField] private StatOverviewUI[] _statUI;
        [SerializeField] private Button _useButton;
        [SerializeField] private Button _equipButton;
        [SerializeField] private Button _unequipButton;
        [SerializeField] private Button _sellButton;
        [SerializeField] private Button[] _closeButton;

        [Header("Broadcasting")]
        [SerializeField] private ItemSOEventChannelSO _equipEquipmentEventChannel;
        [SerializeField] private ItemSOEventChannelSO _unequipEquipmentEventChannel;

        private ItemSO _currentSelectedItem;

        private void OnEnable() => AddListeners();
        private void OnDisable() => RemoveListeners();

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
                    EquipmentSO equipment = item as EquipmentSO;
                    IEquippable equippable = PlayerEquipmentManager.Instance.PlayerEquipmentDatabase.GetEquipmentObjectBySO(equipment);
                    _useButton.gameObject.SetActive(false);
                    if (equippable != null && equippable.IsEquipped)
                    {
                        _equipButton.gameObject.SetActive(false);
                        _unequipButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        _equipButton.gameObject.SetActive(true);
                        _unequipButton.gameObject.SetActive(false);
                    }
                    _sellButton.gameObject.SetActive(true);
                    ShowStats(equipment);
                    break;
                case ItemType.Edible:
                case ItemType.Potion:
                    _useButton.gameObject.SetActive(true);
                    _equipButton.gameObject.SetActive(false);
                    _unequipButton.gameObject.SetActive(false);
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

        private void AddListeners()
        {
            _useButton.onClick.AddListener(OnUseButtonClick);
            _sellButton.onClick.AddListener(OnSellButtonClick);
            _equipButton.onClick.AddListener(OnEquipButtonClick);
            _unequipButton.onClick.AddListener(OnUnequipButtonClick);

            foreach (Button button in _closeButton)
                button.onClick.AddListener(ToggleMenu);
        }

        private void RemoveListeners()
        {
            _useButton.onClick.RemoveListener(OnUseButtonClick);
            _sellButton.onClick.RemoveListener(OnSellButtonClick);
            _equipButton.onClick.RemoveListener(OnEquipButtonClick);
            _unequipButton.onClick.RemoveListener(OnUnequipButtonClick);

            foreach (Button button in _closeButton)
                button.onClick.RemoveListener(ToggleMenu);
        }

        private void ToggleMenu()
        {
            if (_overviewPanel.activeSelf)
                _overviewPanel.SetActive(false);
            else
                _overviewPanel.SetActive(true);
        }

        private void ShowStats(EquipmentSO equipment)
        {
            // First, hide all stat UI elements
            foreach (StatOverviewUI statUI in _statUI)
                statUI.gameObject.SetActive(false);

            EquipmentBaseStatsSO equipmentStats = equipment.equipmentStats.BaseStats;

            _primaryStatTypeText.text = equipmentStats.PrimaryBaseStat.statType.ToString();
            _primaryStatValueText.text = "+" + equipmentStats.PrimaryBaseStat.value.ToString("0.##") + (equipmentStats.PrimaryBaseStat.statValueType
                                                                                                       == StatValueType.Flat ? "" : "%");

            // Then, display only the stats that are relevant
            foreach (ReadonlyBaseStat secondaryBaseStat in equipmentStats.SecondaryBaseStats)
            {
                foreach (StatOverviewUI statUI in _statUI)
                {
                    if (statUI.statType == secondaryBaseStat.statType)
                    {
                        statUI.gameObject.SetActive(true);

                        statUI.statValueText.text = "+" + secondaryBaseStat.value.ToString("0.##") + (secondaryBaseStat.statValueType == StatValueType.Flat ? "" : "%");
                        break;
                    }
                }
            }
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
            {
                _equipEquipmentEventChannel.RaiseEvent(_currentSelectedItem);
                _equipButton.gameObject.SetActive(false);
                _unequipButton.gameObject.SetActive(true);
            }
        }

        private void OnUnequipButtonClick()
        {
            if (_currentSelectedItem != null)
            {
                _unequipEquipmentEventChannel.RaiseEvent(_currentSelectedItem);
                _equipButton.gameObject.SetActive(true);
                _unequipButton.gameObject.SetActive(false);
            }
        }
    }
}