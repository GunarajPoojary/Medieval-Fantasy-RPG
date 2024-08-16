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
    public class InventoryItemOverviewLayout : MonoBehaviour, IItemOverviewDisplay
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _itemDescription;
        [SerializeField] private GameObject _container;
        [SerializeField] private Image _image;
        [Space]
        [SerializeField] private VoidReturnItemSOAndGameObjectParameterEventChannelSO _inventorySlotSelectionEventChannelSO;  // Event channel for item selection
        [Space]
        [SerializeField] private VoidReturnItemSOParameterEventChannelSO _removeItem;  // Event channel to remove items
        [Space]
        private ItemSO _item;
        private GameObject _selectedSlotGameObject;

        private IItemActionHandler _itemActionHandler;

        private void Awake() => _itemActionHandler = GetComponent<ItemActionHandler>();

        private void OnEnable()
        {
            HideDetails();
            _inventorySlotSelectionEventChannelSO.OnEventRaised += HandleSlotSelection;
        }

        private void OnDisable() => _inventorySlotSelectionEventChannelSO.OnEventRaised -= HandleSlotSelection;

        public void DisplayItemOverview(ItemSO item)
        {
            _nameText.text = item.Name;
            _itemDescription.text = item.Description;
            _image.sprite = item.Icon;
            _container.SetActive(true);
        }

        /// <summary>
        /// Hides the item overview UI.
        /// </summary>
        public void HideDetails() => _container.SetActive(false);

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