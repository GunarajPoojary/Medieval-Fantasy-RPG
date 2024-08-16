using RPG.Combat;
using RPG.Core.SaveLoad;
using RPG.Core.Utils;
using RPG.ScriptableObjects;
using RPG.ScriptableObjects.EventChannels;
using RPG.ScriptableObjects.Items;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.EquipmentSystem
{
    /// <summary>
    /// Class that manages equipping and unequipping items for the player, and handles save/load functionality
    /// </summary>
    public class EquipmentManager : SimpleSingleton<EquipmentManager>, ISaveable
    {
        // Serialized event channels for item lookups, equipment changes, and equipping/unequipping items
        [SerializeField] private ItemSOReturnStringParameterEventChannelSO _itemLookupEventSO;
        [Space]
        [SerializeField] private WeaponSOReturnNonParameterEventChannelSO _equippedWeaponEventChannelSO;
        [Space]
        [SerializeField] private WearableSOArrayReturnNonParameterEventChannelSO _equippedWearableEventChannelSO;
        [Space]
        [SerializeField] private VoidReturnDoubleWeaponSOParameterEventChannelSO _weaponChangedEventChannelSO;
        [Space]
        [SerializeField] private VoidReturnDoubleWearableSOParameterEventChannelSO _wearableChangedEventChannelSO;
        [Space]
        [SerializeField] private VoidReturnWeaponSOParameterEventChannelSO _equipWeaponSOEventChannelSO;
        [Space]
        [SerializeField] private WeaponSOReturnNonParameterEventChannelSO _unequipWeaponSOEventChannelSO;
        [Space]
        [SerializeField] private VoidReturnWearableSOParameterEventChannelSO _equipWearableEventChannelSO;
        [Space]
        [SerializeField] private WearableSOReturnIntParameterEventChannelSO _unequipWearableEventChannelSO;
        [Space]
        [SerializeField] private RuntimeAnimatorController _defaultAnimatorController;

        private const string PLAYER_TAG = "Player";

        private WeaponSO _equippedWeaponSO;
        private WearableSO[] _equippedWearableSO;

        private SkinnedMeshRenderer _playerHeadMesh;
        private Transform _playerVisualTransform;

        private Animator _playerAnimator;

        private PlayerEquipments _playerEquipments;

        private List<GameObject> _equippedWeaponGameobjects;

        private SkinnedMeshRenderer[] _equippedWearableMeshes;

        private int _sceneBuildIndex;

        // Interfaces for setting default skins and activating skins
        private IDefaultSkinSetter _defaultSkinSetter;
        private ISkinsActivator _skinsActivator;

        protected override void Awake()
        {
            base.Awake();
            InitializeEquipmentSlots();
        }

        private void OnEnable()
        {
            _equippedWeaponEventChannelSO.OnEventRaised += HandleEquippedWeaponRequest;
            _equippedWearableEventChannelSO.OnEventRaised += HandleEquippedWearableRequests;
            _equipWeaponSOEventChannelSO.OnEventRaised += EquipWeapon;
            _unequipWeaponSOEventChannelSO.OnEventRaised += UnequipWeapon;
            _equipWearableEventChannelSO.OnEventRaised += EquipWearable;
            _unequipWearableEventChannelSO.OnEventRaised += UnequipWearable;

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            _equippedWeaponEventChannelSO.OnEventRaised -= HandleEquippedWeaponRequest;
            _equippedWearableEventChannelSO.OnEventRaised -= HandleEquippedWearableRequests;
            _equipWeaponSOEventChannelSO.OnEventRaised -= EquipWeapon;
            _unequipWeaponSOEventChannelSO.OnEventRaised -= UnequipWeapon;
            _equipWearableEventChannelSO.OnEventRaised -= EquipWearable;
            _unequipWearableEventChannelSO.OnEventRaised -= UnequipWearable;

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // Equip a new wearable item, unequip the old one if necessary, and update the visuals
        public void EquipWearable(WearableSO newItem)
        {
            int slotIndex = (int)newItem.EquipSlot;
            WearableSO oldItem = UnequipWearable(slotIndex);

            _defaultSkinSetter.SetDefaultSkin(slotIndex, false);

            _equippedWearableSO[slotIndex] = newItem;
            var newMesh = Instantiate(newItem.SkinnedMesh, _playerVisualTransform);
            newMesh.bones = _playerHeadMesh.bones;
            newMesh.rootBone = _playerHeadMesh.rootBone;
            _equippedWearableMeshes[slotIndex] = newMesh;

            _wearableChangedEventChannelSO.RaiseEvent(newItem, oldItem);
        }

        // Unequip a wearable item from a specific slot, destroy the associated mesh, and reset the default skin
        public WearableSO UnequipWearable(int slotIndex)
        {
            var oldItem = _equippedWearableSO[slotIndex];

            if (_equippedWearableMeshes[slotIndex] != null)
            {
                Destroy(_equippedWearableMeshes[slotIndex].gameObject);
            }

            _equippedWearableSO[slotIndex] = null;

            if (_equippedWearableSO[slotIndex] == null)
            {
                _defaultSkinSetter.SetDefaultSkin(slotIndex, true);
            }
            return oldItem;
        }

        // Equip a new weapon, unequip the old one if necessary, and update the visuals and animator controller
        public void EquipWeapon(WeaponSO newItem)
        {
            WeaponSO oldItem = UnequipWeapon();

            _equippedWeaponSO = newItem;
            EquipWeaponPrefabs(newItem);
            UpdateAnimatorController(newItem);

            _weaponChangedEventChannelSO.RaiseEvent(newItem, oldItem);
        }

        // Unequip the currently equipped weapon, destroy its game objects, and reset the animator controller
        public WeaponSO UnequipWeapon()
        {
            if (_equippedWeaponSO == null)
            {
                return null;
            }

            if (_equippedWeaponGameobjects != null)
            {
                foreach (var weapon in _equippedWeaponGameobjects)
                {
                    if (weapon != null)
                    {
                        Destroy(weapon);
                    }
                }
            }

            var oldItem = _equippedWeaponSO;

            _equippedWeaponSO = null;

            AnimatorOverrideController overrideController = _playerAnimator.runtimeAnimatorController as AnimatorOverrideController;
            _playerAnimator.runtimeAnimatorController = _defaultAnimatorController;

            return oldItem;
        }

        // Unequip all wearable items and weapons, and reset the player to the default skins and animator controller
        public void UnequipAll()
        {
            if (_equippedWearableSO != null)
            {
                for (int i = 0; i < _equippedWearableSO.Length; i++)
                {
                    UnequipWearable(i);
                }
            }

            UnequipWeapon();

            EquipDefaultSkins();

            _playerAnimator.runtimeAnimatorController = _defaultAnimatorController;
        }

        #region ISaveable Methods
        public void LoadData(GameData data)
        {
            if (_itemLookupEventSO.RaiseEvent(data.CurrentWeaponObjectIDs) is WeaponSO weapon)
            {
                _equippedWeaponSO = weapon;
            }

            for (int i = 0; i < data.CurrentArmorObjectIDs.Length; i++)
            {
                if (_itemLookupEventSO.RaiseEvent(data.CurrentArmorObjectIDs[i]) is WearableSO armor)
                {
                    _equippedWearableSO[i] = armor;
                }
            }
        }

        public void SaveData(GameData data)
        {
            if (_equippedWeaponSO != null)
            {
                data.CurrentWeaponObjectIDs = _equippedWeaponSO.ID;
            }
            else
            {
                data.CurrentWeaponObjectIDs = null;
            }

            data.CurrentArmorObjectIDs = new string[_equippedWearableSO.Length];

            for (int i = 0; i < _equippedWearableSO.Length; i++)
            {
                if (_equippedWearableSO[i] != null)
                {
                    data.CurrentArmorObjectIDs[i] = _equippedWearableSO[i].ID;
                }
            }
        }
        #endregion

        // Instantiate and equip the weapon prefabs to the player's hands
        private void EquipWeaponPrefabs(WeaponSO newItem)
        {
            _equippedWeaponGameobjects = new List<GameObject>();

            foreach (var gameObject in newItem.WeaponPrefabs)
            {
                if (_playerEquipments.Equipments.TryGetValue(gameObject, out var weaponHandTransform))
                {
                    var newWeapon = Instantiate(gameObject, weaponHandTransform);
                    newWeapon.transform.localPosition = Vector3.zero;
                    newWeapon.transform.localRotation = Quaternion.identity;
                    _equippedWeaponGameobjects.Add(newWeapon);
                }
            }
        }

        // Update the animator controller based on the equipped weapon and current scene
        private void UpdateAnimatorController(WeaponSO newItem)
        {
            AnimatorOverrideController overrideController = _playerAnimator.runtimeAnimatorController as AnimatorOverrideController;
            if (_sceneBuildIndex == 2)
            {
                _playerAnimator.runtimeAnimatorController = newItem.CharacterMenuAnimatorOverrideController != null ? newItem.CharacterMenuAnimatorOverrideController : _defaultAnimatorController;
            }
            else if (_sceneBuildIndex == 1)
            {
                _playerAnimator.runtimeAnimatorController = newItem.GameplayAnimatorOverrideController != null ? newItem.GameplayAnimatorOverrideController : _defaultAnimatorController;
            }
        }

        // Handle requests to get the currently equipped wearable items
        private WearableSO[] HandleEquippedWearableRequests() => _equippedWearableSO;

        // Handle requests to get the currently equipped weapon
        private WeaponSO HandleEquippedWeaponRequest() => _equippedWeaponSO;

        // Handle scene loading events, re-initialize player equipment when a new scene is loaded
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            _sceneBuildIndex = scene.buildIndex;

            if (scene.buildIndex != 0)
            {
                PlayerInitialization();
            }
            else
            {
                return;
            }

            if (_equippedWeaponSO != null)
            {
                EquipWeapon(_equippedWeaponSO);
            }

            if (_equippedWearableSO != null)
            {
                for (int i = 0; i < _equippedWearableSO.Length; i++)
                {
                    if (_equippedWearableSO[i] != null)
                    {
                        EquipWearable(_equippedWearableSO[i]);
                    }
                }
            }
        }

        private void PlayerInitialization()
        {
            GameObject player = GameObject.FindWithTag(PLAYER_TAG);

            _playerAnimator = player.GetComponentInChildren<Animator>();

            _playerVisualTransform = _playerAnimator.transform;

            _playerHeadMesh = _playerVisualTransform.GetChild(1).GetComponent<SkinnedMeshRenderer>();

            _playerEquipments = player.GetComponentInChildren<PlayerEquipments>();

            _defaultSkinSetter = player.GetComponent<PlayerDefaultSkins>();
            _skinsActivator = player.GetComponent<PlayerDefaultSkins>();
        }

        // Initialize arrays for storing equipped wearables and their corresponding meshes
        private void InitializeEquipmentSlots()
        {
            int numSlots = Enum.GetNames(typeof(WearableSlot)).Length;
            _equippedWearableSO = new WearableSO[numSlots];
            _equippedWearableMeshes = new SkinnedMeshRenderer[numSlots];
        }

        // Activate the default skins for all slots
        private void EquipDefaultSkins() => _skinsActivator.ActivateSkins();
    }
}