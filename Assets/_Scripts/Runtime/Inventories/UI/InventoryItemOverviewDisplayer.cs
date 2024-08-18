using RPG.ScriptableObjects.EventChannels;
using RPG.ScriptableObjects.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Inventories.UI
{
    /// <summary>
    /// Displays detailed information about a selected Item in the inventory.
    /// </summary>
    public class InventoryItemOverviewDisplayer : MonoBehaviour, IItemOverviewDisplayer
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _itemDescription;
        [SerializeField] private GameObject _overviewContainer;
        [SerializeField] private Image _image;
        [Space]
        [SerializeField] private VoidReturnItemSOAndGameObjectParameterEventChannelSO _inventorySlotSelectionEventChannelSO;  // Event channel for item selection

        private ItemSO _item;

        private GameObject _selectedSlotGameObject;

        private IInventoryItemActionHandler _itemActionHandler;

        private void Awake() => _itemActionHandler = GetComponent<InventoryItemActionHandler>();

        private void OnEnable()
        {
            HideOverviewLayout();
            _inventorySlotSelectionEventChannelSO.OnEventRaised += HandleSlotSelection;
        }

        private void OnDisable() => _inventorySlotSelectionEventChannelSO.OnEventRaised -= HandleSlotSelection;

        #region IItemOverviewDisplay Methods
        public void DisplayItemOverview(ItemSO item)
        {
            _nameText.text = item.Name;
            _itemDescription.text = item.Description;
            _image.sprite = item.Icon;
            _overviewContainer.SetActive(true);
        }

        /// <summary>
        /// Hides the item overview UI.
        /// </summary>
        public void HideOverviewLayout() => _overviewContainer.SetActive(false);
        #endregion

        public void SellItem() => _itemActionHandler.SellItem(_item, _selectedSlotGameObject);

        public void EnhanceItem() => _itemActionHandler.EnhanceItem(_item);

        public void UseItem() => _itemActionHandler.UseItem(_item);

        private void HandleSlotSelection(ItemSO item, GameObject slot)
        {
            _item = item;
            _selectedSlotGameObject = slot;
            DisplayItemOverview(item);
        }
    }
}