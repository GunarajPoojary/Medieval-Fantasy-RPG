﻿using AYellowpaper.SerializedCollections;
using RPG.EquipmentSystem;
using RPG.Inventories.UI;
using RPG.ScriptableObjects.Items;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.CharacterMenu
{
    /// <summary>
    /// Manages the overview of wearable equipment in the character menu.
    /// Handles the display of armor slots and adds new equipment to the corresponding slots.
    /// </summary>
    public class WearablesOverviewHandler : MonoBehaviour, IEquipmentOverviewDisplayer, IEquipmentAdder
    {
        [SerializedDictionary("Armor Slot", "Content Transform")]
        [SerializeField] private SerializedDictionary<WearableSlot, Transform> _armorSlotsContentMap = new SerializedDictionary<WearableSlot, Transform>(6);

        [SerializeField] private GameObject _slotPrefab;

        private Dictionary<ItemSO, GameObject> _itemObjectToGameobjectMap = new Dictionary<ItemSO, GameObject>();

        #region IWeaponOverviewDisplayer Method
        // Displays the overview of the first active armor slot.
        public void DisplayEquipmentOverview()
        {
            List<EquipmentSlotUI> wearableSlots = new List<EquipmentSlotUI>();

            // Gather all armor slots that are currently active.
            foreach (var slot in _armorSlotsContentMap.Values)
            {
                wearableSlots.AddRange(slot.GetComponentsInChildren<EquipmentSlotUI>());
            }

            if (wearableSlots.Count != 0)
            {
                // Display the overview for the first active slot.
                for (int i = 0; i < wearableSlots.Count; i++)
                {
                    if (wearableSlots[i].gameObject.activeSelf)
                    {
                        wearableSlots[i].DisplayItemOverview();
                        return;
                    }
                }
            }
        }
        #endregion

        #region IEquipmentAddable Method
        // Adds a new piece of equipment to the appropriate armor slot.
        public void AddEquipment(EquipmentSO equipment)
        {
            WearableSO wearable = (WearableSO)equipment;

            if (_itemObjectToGameobjectMap.ContainsKey(wearable))
            {
                return;
            }

            // Check if there's a corresponding slot for the wearable.
            if (_armorSlotsContentMap.TryGetValue(wearable.EquipSlot, out Transform slotTransform))
            {
                var slot = Instantiate(_slotPrefab, slotTransform);
                IItemSetter itemSlot = slot.GetComponent<EquipmentSlotUI>();
                itemSlot.SetItem(wearable);
                _itemObjectToGameobjectMap.Add(wearable, slot);
            }
        }
        #endregion
    }
}