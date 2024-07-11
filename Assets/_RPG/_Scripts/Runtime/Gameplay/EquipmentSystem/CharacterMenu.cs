using AYellowpaper.SerializedCollections;
using RPG.Gameplay.Inventories;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RPG.Gameplay.EquipmentSystem
{
    /// <summary>
    /// Manages the character menu functionality, including displaying equipped items.
    /// </summary>
    public class CharacterMenu : MonoBehaviour
    {
        [SerializedDictionary("Armor Slot", "Content Transform")]
        [SerializeField] private SerializedDictionary<ArmorSlot, Transform> _armorSlotsContentMap = new(6);
        [SerializeField] private Transform _weaponsContainer;

        [SerializeField] private GameObject _slotPrefab;

        private Dictionary<ItemSO, GameObject> _itemObjectToGameobjectMap = new();

        [SerializeField] private GameObject _weaponSelectionContainer;
        [SerializeField] private GameObject _armorSelectionContainer;
        [SerializeField] private GameObject _MainPanelContainer;

        public event Action OnEnterMainPanel;
        public event Action OnEnterEquipmentSelection;

        private void Awake()
        {
            foreach (var item in Inventory.Instance.Items)
                AddItems(item);
        }

        public void Switch(GameObject equipmentSelectionScreen)
        {
            if (equipmentSelectionScreen == _weaponSelectionContainer)
            {
                ShowRandomWeaponInfo();
            }
            else if (equipmentSelectionScreen == _armorSelectionContainer)
            {
                ShowRandomArmorInfo();
            }
            else if (equipmentSelectionScreen == _MainPanelContainer)
            {
                SetPanels(mainPanelSetActive: true, weaponSelectionSetActive: false, armorSelectionSetActive: false);
                OnEnterMainPanel?.Invoke();
            }
        }

        private void ShowRandomWeaponInfo()
        {
            List<EquipmentSlot> weaponSlots = new List<EquipmentSlot>();
            weaponSlots = _weaponsContainer.GetComponentsInChildren<EquipmentSlot>().ToList();

            if (weaponSlots.Count != 0)
            {
                SetPanels(mainPanelSetActive: false, weaponSelectionSetActive: true, armorSelectionSetActive: false);
                weaponSlots[UnityEngine.Random.Range(0, weaponSlots.Count)].ShowItemInfo();
                OnEnterEquipmentSelection?.Invoke();
            }
            else
            {
                Debug.LogWarning("You don't have any weapon to equip");
                return;
            }
        }

        private void ShowRandomArmorInfo()
        {
            List<EquipmentSlot> armorSlots = new List<EquipmentSlot>();

            for (int i = 0; i < 6; i++)
            {
                if (_armorSlotsContentMap.TryGetValue((ArmorSlot)i, out Transform content))
                    armorSlots.AddRange(content.GetComponentsInChildren<EquipmentSlot>().ToList());
            }

            if (armorSlots.Count != 0)
            {
                SetPanels(mainPanelSetActive: false, weaponSelectionSetActive: false, armorSelectionSetActive: true);
                armorSlots[UnityEngine.Random.Range(0, armorSlots.Count)].ShowItemInfo();
                OnEnterEquipmentSelection?.Invoke();
            }
            else
            {
                Debug.LogWarning("You don't have any armor to equip");
                return;
            }
        }

        private void SetPanels(bool mainPanelSetActive, bool weaponSelectionSetActive, bool armorSelectionSetActive)
        {
            _MainPanelContainer.SetActive(mainPanelSetActive);
            _weaponSelectionContainer.SetActive(weaponSelectionSetActive);
            _armorSelectionContainer.SetActive(armorSelectionSetActive);
        }

        private void AddItems(ItemSO item)
        {
            if (item is WeaponSO weaponItem)
            {
                AddEquipmentToContainer(_weaponsContainer, weaponItem);
            }
            else if (item is ArmorSO armorItem)
            {
                if (_armorSlotsContentMap.TryGetValue(armorItem.ArmorEquipSlot, out Transform slotTransform))
                    AddEquipmentToContainer(slotTransform, armorItem);
            }
        }

        private void AddEquipmentToContainer(Transform container, EquipmentSO item)
        {
            // Check if the item is already in the dictionary
            if (_itemObjectToGameobjectMap.ContainsKey(item))
            {
                Debug.LogWarning($"Item '{item.name}' is already in the container.");
                return;
            }

            var slot = Instantiate(_slotPrefab, container);
            EquipmentSlot itemSlot = slot.GetComponent<EquipmentSlot>();
            itemSlot.SetItem(item);
            _itemObjectToGameobjectMap.Add(item, slot);
        }
    }
}
