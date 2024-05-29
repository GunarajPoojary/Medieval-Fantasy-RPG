using Cinemachine;
using GunarajCode.Inventories;
using GunarajCode.ScriptableObjects;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GunarajCode
{
    /// <summary>
    /// Manages the character menu functionality, including displaying equipped items.
    /// </summary>
    public class CharacterMenu : MonoBehaviour
    {
        // Container class for armor slots and their corresponding UI transforms
        [System.Serializable]
        public class ArmorSlotContainer
        {
            public ArmorSlot SlotType;
            public Transform Content;
        }

        private PlayerInputsAction _inputActions;
        private Inventory _inventory;

        [SerializeField] private CharacterScreenItemInfo _characterScreenItemInfo;
        [SerializeField] private List<ArmorSlotContainer> _armorSlotsContainer;
        [SerializeField] private Transform _weaponsContainer;
        [SerializeField] private GameObject _slotPrefab;
        [SerializeField] private GameObject _characterMenuContainer;
        [SerializeField] private CinemachineFreeLook _thirdPersonCam;
        public Dictionary<ItemObject, GameObject> _itemObjectToGameobjectMap = new Dictionary<ItemObject, GameObject>();

        // Dictionary to map armor slots to their corresponding UI transforms
        private readonly Dictionary<ArmorSlot, Transform> _armorSlotToContentUIMap = new Dictionary<ArmorSlot, Transform>();

        private void Awake()
        {
            _inputActions = new PlayerInputsAction();
            InitializeArmorSlotUI();
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();
            _inputActions.Player.CharacterMenu.performed += ToggleCharacterMenu;

            _inventory = Inventory.Instance;
            _inventory.OnItemAdded += OnItemAdded;
            _inventory.OnItemRemoved += OnItemRemoved;
        }

        private void OnItemRemoved(ItemObject item)
        {
            if (_itemObjectToGameobjectMap.TryGetValue(item, out GameObject value))
            {
                Destroy(value);
                _itemObjectToGameobjectMap.Remove(item);
            }
        }

        private void OnDisable()
        {
            _inputActions.Player.CharacterMenu.performed -= ToggleCharacterMenu;
            _inputActions.Player.Disable();

            _inventory.OnItemAdded -= OnItemAdded;
            _inventory.OnItemRemoved -= OnItemRemoved;
        }

        private void InitializeArmorSlotUI() => _armorSlotsContainer.ForEach(slot => _armorSlotToContentUIMap[slot.SlotType] = slot.Content);

        private void Start() => _characterMenuContainer.SetActive(false);

        private void ToggleCharacterMenu(InputAction.CallbackContext context)
        {
            bool isActive = _characterMenuContainer.activeSelf;
            _characterMenuContainer.SetActive(!isActive);
            _thirdPersonCam.Priority = isActive ? 10 : 0;
        }

        private void OnItemAdded(ItemObject item)
        {
            if (item is WeaponObject weaponItem)
            {
                AddItemToContainer(_weaponsContainer, weaponItem);
            }
            else if (item is ArmorObject armorItem)
            {
                if (_armorSlotToContentUIMap.TryGetValue(armorItem.ArmorEquipSlot, out Transform slotTransform))
                    AddItemToContainer(slotTransform, armorItem);
            }
        }

        private void AddItemToContainer(Transform container, ItemObject item)
        {
            var slot = Instantiate(_slotPrefab, container);
            ItemSlot itemSlot = slot.GetComponent<ItemSlot>();
            itemSlot.SetItem(item);
            _itemObjectToGameobjectMap.Add(item, slot);
            itemSlot.OnButtonPressed += _characterScreenItemInfo.OnButtonPressed;

        }
    }
}
