using RPG.EquipmentSystem;
using RPG.Inventories.UI;
using RPG.ScriptableObjects.Items;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.CharacterMenu
{
    /// <summary>
    /// Manages the overview of weapon equipment in the character menu.
    /// Handles the display of weapon slots and adds new equipment to the corresponding slots.
    /// </summary>
    public class WeaponOverviewHandler : MonoBehaviour, IEquipmentOverviewDisplayer, IEquipmentAddable
    {
        [SerializeField] private Transform _weaponsContainer;
        [SerializeField] private GameObject _slotPrefab;

        private Dictionary<ItemSO, GameObject> _itemObjectToGameobjectMap = new Dictionary<ItemSO, GameObject>();

        #region IEquipmentOverviewDisplayer Method
        // Displays the overview of a random active weapon slot.
        public void DisplayEquipmentOverview()
        {
            List<EquipmentSlotUI> weaponSlots = _weaponsContainer.GetComponentsInChildren<EquipmentSlotUI>().ToList();

            if (weaponSlots.Count != 0)
            {
                weaponSlots[Random.Range(0, weaponSlots.Count)].DisplayItemOverview();
            }
        }
        #endregion

        #region IEquipmentAddable Method
        // Adds a new weapon to the appropriate slot in the weapons container.
        public void AddEquipment(EquipmentSO equipment)
        {
            WeaponSO weapon = (WeaponSO)equipment;

            if (_itemObjectToGameobjectMap.ContainsKey(weapon))
            {
                return;
            }

            // Instantiate a new slot for the weapon and add it to the container.
            var slot = Instantiate(_slotPrefab, _weaponsContainer);
            IItemSetter itemSlot = slot.GetComponent<EquipmentSlotUI>();
            itemSlot.SetItem(weapon);
            _itemObjectToGameobjectMap.Add(weapon, slot);
        }
        #endregion
    }
}