using GunarajCode.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GunarajCode
{
    public class ItemInfoUI : MonoBehaviour
    {
        protected ItemObject _item;
        protected GameObject _slotPrefab;

        [SerializeField] protected GameObject _container;
        [SerializeField] protected TextMeshProUGUI _itemName;
        [SerializeField] protected Image _image;
        [SerializeField] protected TextMeshProUGUI _itemDescription;
        [SerializeField] protected TextMeshProUGUI _itemStats;

        /// <summary>
        /// Called when the item button is pressed. It updates the UI with the selected item's information
        /// and shows the item details.
        /// </summary>
        /// <param name="item">The item to display.</param>
        /// <param name="slot">The inventory slot associated with the item.</param>
        public void OnButtonPressed(ItemObject item, GameObject slot)
        {
            _item = item;
            _slotPrefab = slot;

            UpdateItemInfo();

            ShowInfo();
        }

        private void UpdateItemInfo()
        {
            _itemName.text = _item.DisplayName;
            _image.sprite = _item.Icon;
            _itemDescription.text = _item.Description;

            // Update the item stats if the item is an equipment.
            if (_item is EquipmentObject equipment)
                ShowEquipmentStats(equipment);
            else
                _itemStats.text = string.Empty;
        }

        private void ShowEquipmentStats(EquipmentObject equipment) => _itemStats.text = equipment.EquipmentStats.ToString();

        public void HideInfo() => _container.SetActive(false);

        private void ShowInfo()
        {
            if (!_container.activeSelf)
                _container.SetActive(true);
        }
    }
}
