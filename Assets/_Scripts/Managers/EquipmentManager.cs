using GunarajCode.Inventories;
using GunarajCode.ScriptableObjects;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GunarajCode
{
    /// <summary>
    /// Manages the equipment system for the player, including armor and weapons.
    /// Handles equipping, unequipping, and initializing default equipment.
    /// Inherits from a Singleton base class to ensure only one instance exists.
    /// </summary>
    public class EquipmentManager : Singleton<EquipmentManager>
    {
        /// <summary>
        /// Represents a pair of weapon prefab and its corresponding hand transform to attach and detach during runtime, 
        /// when equipped or unequipped.
        /// </summary>
        [Serializable]
        public class WeaponHandTransform
        {
            public GameObject WeaponPrefab;
            public Transform HandTransform;
        }

        private Inventory _inventory;

        // Player's head mesh renderer used to set armors(SkinnedMeshRenderer) bones
        [SerializeField] private SkinnedMeshRenderer _playerHeadMesh;
        // Transform of the player's visual for setting armor's parent
        [SerializeField] private Transform _playerVisual;
        // Default armor objects which are used when there are no armors
        [SerializeField] private ArmorObject[] _defaultBodyObjects;
        // Default armor prefabs to be destroyed when the game starts(They are used only for editor)
        [SerializeField] private GameObject[] _defaultBodyPrefabs;
        [SerializeField] private Animator _animator;
        [SerializeField] private List<WeaponHandTransform> _weaponsHandTransforms = new List<WeaponHandTransform>();
        [SerializeField] private KeyCode _unequipAllKey = KeyCode.U;

        private SkinnedMeshRenderer[] _currentArmorMeshes;
        private ArmorObject[] _currentArmorObjects;
        private WeaponObject _currentWeaponObject;
        private List<GameObject> _currentWeapons = new List<GameObject>();
        private Dictionary<GameObject, Transform> _weaponToHandTransformMap = new Dictionary<GameObject, Transform>();

        private RuntimeAnimatorController _defaultAnimatorController;

        public event Action<EquipmentObject, EquipmentObject> OnEquipmentChanged;

        private void OnEnable()
        {
            _inventory = Inventory.Instance;
            InitializeWeaponHandTransformMap();
        }

        private void InitializeWeaponHandTransformMap() => _weaponsHandTransforms.ForEach(x => _weaponToHandTransformMap[x.WeaponPrefab] = x.HandTransform);

        private void Start()
        {
            DestroyDefaultBodyPrefabs();
            _defaultAnimatorController = _animator.runtimeAnimatorController;

            InitializeEquipmentSlots();
            EquipDefaultSkins();
        }

        private void DestroyDefaultBodyPrefabs()
        {
            foreach (var obj in _defaultBodyPrefabs)
                Destroy(obj);
        }

        private void InitializeEquipmentSlots()
        {
            int numSlots = Enum.GetNames(typeof(ArmorSlot)).Length;
            _currentArmorObjects = new ArmorObject[numSlots];
            _currentArmorMeshes = new SkinnedMeshRenderer[numSlots];
        }

        private void Update()
        {
            if (Input.GetKeyDown(_unequipAllKey))
                UnequipAll();
        }

        /// <summary>
        /// Equips an item, either armor or weapon.
        /// </summary>
        /// <param name="item">The item to be equipped.</param>
        public void Equip(EquipmentObject item)
        {
            if (item is ArmorObject armor)
            {
                EquipArmor(armor);
            }
            else if (item is WeaponObject weapon)
            {
                EquipWeapon(weapon);
            }
        }

        private void EquipArmor(ArmorObject newItem)
        {
            int slotIndex = (int)newItem.ArmorEquipSlot;
            ArmorObject oldItem = UnequipArmor(slotIndex);

            if (_currentArmorObjects[slotIndex] != null)
            {
                oldItem = _currentArmorObjects[slotIndex];

                if (!oldItem.IsDefault)
                    _inventory.Add(oldItem);
            }

            _currentArmorObjects[slotIndex] = newItem;
            var newMesh = Instantiate(newItem.SkinnedMesh, _playerVisual);
            newMesh.bones = _playerHeadMesh.bones;
            newMesh.rootBone = _playerHeadMesh.rootBone;
            _currentArmorMeshes[slotIndex] = newMesh;

            OnEquipmentChanged?.Invoke(newItem, oldItem);
        }

        private void EquipWeapon(WeaponObject newItem)
        {
            WeaponObject oldItem = UnequipWeapon();

            if (_currentWeaponObject != null)
            {
                oldItem = _currentWeaponObject;

                _inventory.Add(oldItem);
            }

            _currentWeaponObject = newItem;
            EquipWeaponPrefabs(newItem);
            UpdateAnimatorController(newItem);

            OnEquipmentChanged?.Invoke(newItem, oldItem);
        }

        // Equips the weapon prefabs to the corresponding hand transforms.
        private void EquipWeaponPrefabs(WeaponObject newItem)
        {
            foreach (var gameObject in newItem.WeaponPrefabs)
            {
                if (_weaponToHandTransformMap.TryGetValue(gameObject, out var weaponHandTransform))
                {
                    if (gameObject.TryGetComponent<ItemPickUp>(out ItemPickUp item))
                        item.enabled = false;

                    var newWeapon = Instantiate(gameObject, weaponHandTransform);
                    newWeapon.transform.localPosition = Vector3.zero;
                    newWeapon.transform.localRotation = Quaternion.identity;
                    _currentWeapons.Add(newWeapon);
                }
            }
        }

        // Updates the animator controller based on the new weapon.
        private void UpdateAnimatorController(WeaponObject newItem)
        {
            var overrideController = _animator.runtimeAnimatorController as AnimatorOverrideController;
            if (newItem.AnimatorOverride != null)
                _animator.runtimeAnimatorController = newItem.AnimatorOverride;
            else if (overrideController != null)
                _animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
        }

        /// <summary>
        /// Unequips the armor from a specific slot.
        /// </summary>
        /// <param name="slotIndex">The index of the slot to unequip.</param>
        /// <returns>The unequipped armor item.</returns>
        public ArmorObject UnequipArmor(int slotIndex)
        {
            if (_currentArmorObjects[slotIndex] == null)
                return null;

            var oldItem = _currentArmorObjects[slotIndex];
            if (_currentArmorMeshes[slotIndex] != null)
                Destroy(_currentArmorMeshes[slotIndex].gameObject);

            _currentArmorObjects[slotIndex] = null;
            if (!oldItem.IsDefault)
                _inventory.Add(oldItem);

            OnEquipmentChanged?.Invoke(null, oldItem);
            return oldItem;
        }

        /// <summary>
        /// Unequips the currently equipped weapon.
        /// </summary>
        /// <returns>The unequipped weapon item.</returns>
        public WeaponObject UnequipWeapon()
        {
            if (_currentWeaponObject == null)
                return null;

            foreach (var weapon in _currentWeapons)
            {
                if (weapon != null)
                    Destroy(weapon);
            }

            var oldItem = _currentWeaponObject;
            _inventory.Add(oldItem);

            _currentWeaponObject = null;
            OnEquipmentChanged?.Invoke(null, oldItem);
            return oldItem;
        }

        /// <summary>
        /// Unequips all equipped items and equips the default skins.
        /// </summary>
        public void UnequipAll()
        {
            for (int i = 0; i < _currentArmorObjects.Length; i++)
                UnequipArmor(i);

            UnequipWeapon();

            EquipDefaultSkins();

            _animator.runtimeAnimatorController = _defaultAnimatorController;
        }

        private void EquipDefaultSkins()
        {
            foreach (var item in _defaultBodyObjects)
                Equip(item);
        }
    }
}
