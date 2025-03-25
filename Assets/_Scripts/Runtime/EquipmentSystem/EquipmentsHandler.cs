using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG
{
    /// <summary>
    /// Class that manages equipping and unequipping items for the player, and handles save/load functionality
    /// </summary>
    public class EquipmentsHandler : MonoBehaviour//, ISaveable
    {
        [SerializedDictionary("Weapon Prefab", "Hand Transform")]
        [field: SerializeField] public SerializedDictionary<GameObject, Transform> WeaponToHandTransformMap { get; private set; }

        [field: SerializeField] public SerializedDictionary<WearableSlot, GameObject> Skins { get; set; }

        [SerializeField] private RuntimeAnimatorController _defaultAnimatorController;
        [Space]
        private SkinnedMeshRenderer _playerHeadMesh;
        [Space]
        // Serialized event channels for item lookups, equipment changes, and equipping/unequipping items
        //[SerializeField] private ItemSOReturnStringParameterEventChannelSO _itemLookupEventSO;
        [Space]
        //[SerializeField] private WeaponSOReturnNonParameterEventChannelSO _equippedWeaponEventChannelSO;
        [Space]
        //[SerializeField] private WearableSOArrayReturnNonParameterEventChannelSO _equippedWearableEventChannelSO;
        [Space]
        //[SerializeField] private VoidReturnDoubleWeaponSOParameterEventChannelSO _weaponChangedEventChannelSO;
        [Space]
        //[SerializeField] private VoidReturnDoubleWearableSOParameterEventChannelSO _wearableChangedEventChannelSO;
        [Space]
        //[SerializeField] private VoidReturnWeaponSOParameterEventChannelSO _equipWeaponSOEventChannelSO;
        [Space]
        //[SerializeField] private WeaponSOReturnNonParameterEventChannelSO _unequipWeaponSOEventChannelSO;
        [Space]
        //[SerializeField] private VoidReturnWearableSOParameterEventChannelSO _equipWearableEventChannelSO;
        [Space]
        //[SerializeField] private WearableSOReturnIntParameterEventChannelSO _unequipWearableEventChannelSO;
        [Space]
        //[SerializeField] private IntReturnNonParameterEventChannelSO _getCurrentSceneIndexEventChannelSO;

        private WeaponSO _equippedWeaponSO;
        private WearableSO[] _equippedWearablesSO;
        private Transform _playerVisualTransform;

        private Animator _playerAnimator;

        private List<GameObject> _equippedWeaponGameobjects = new List<GameObject>();

        private SkinnedMeshRenderer[] _equippedWearableMeshes;

        private void Awake()
        {
            InitializeEquipmentSlots();
            PlayerInitialization();
        }

        // Ensure that the Head and Back slots do not have default skins
        private void OnValidate()
        {
            if (Skins.ContainsKey(WearableSlot.Head) || Skins.ContainsKey(WearableSlot.Back))
            {
                Skins.Remove(WearableSlot.Head);
                Skins.Remove(WearableSlot.Back);
            }
        }

        private void OnEnable()
        {
            // _equippedWeaponEventChannelSO.OnEventRaised += GetEquippedWeapon;
            // _equippedWearableEventChannelSO.OnEventRaised += GetEquippedWearables;
            // _equipWeaponSOEventChannelSO.OnEventRaised += EquipWeapon;
            // _unequipWeaponSOEventChannelSO.OnEventRaised += UnequipWeapon;
            // _equipWearableEventChannelSO.OnEventRaised += EquipWearable;
            // _unequipWearableEventChannelSO.OnEventRaised += UnequipWearable;
        }

        private void OnDestroy()
        {
            // _equippedWeaponEventChannelSO.OnEventRaised -= GetEquippedWeapon;
            // _equippedWearableEventChannelSO.OnEventRaised -= GetEquippedWearables;
            // _equipWeaponSOEventChannelSO.OnEventRaised -= EquipWeapon;
            // _unequipWeaponSOEventChannelSO.OnEventRaised -= UnequipWeapon;
            // _equipWearableEventChannelSO.OnEventRaised -= EquipWearable;
            // _unequipWearableEventChannelSO.OnEventRaised -= UnequipWearable;
        }

        private void Start()
        {
            if (_equippedWeaponSO != null)
            {
                EquipWeapon(_equippedWeaponSO);
            }

            if (_equippedWearablesSO != null)
            {
                for (int i = 0; i < _equippedWearablesSO.Length; i++)
                {
                    if (_equippedWearablesSO[i] != null)
                    {
                        EquipWearable(_equippedWearablesSO[i]);
                    }
                }
            }
        }

        // Equip a new wearable item, unequip the old one if necessary, and update the visuals
        public void EquipWearable(WearableSO newItem)
        {
            int slotIndex = (int)newItem.EquipSlot;
            WearableSO oldItem = UnequipWearable(slotIndex);

            ToggleIndividualSkin(slotIndex, false);

            // _equippedWearablesSO[slotIndex] = newItem;
            // var newMesh = Instantiate(newItem.SkinnedMesh, _playerVisualTransform);
            // newMesh.bones = _playerHeadMesh.bones;
            // newMesh.rootBone = _playerHeadMesh.rootBone;
            // _equippedWearableMeshes[slotIndex] = newMesh;
            //
            // _wearableChangedEventChannelSO.RaiseEvent(newItem, oldItem);
        }

        // Unequip a wearable item from a specific slot, destroy the associated mesh, and reset the default skin
        public WearableSO UnequipWearable(int slotIndex)
        {
            var oldItem = _equippedWearablesSO[slotIndex];

            if (_equippedWearableMeshes[slotIndex] != null)
            {
                Destroy(_equippedWearableMeshes[slotIndex].gameObject);
            }

            _equippedWearablesSO[slotIndex] = null;

            if (_equippedWearablesSO[slotIndex] == null)
            {
                ToggleIndividualSkin(slotIndex, true);
            }
            return oldItem;
        }

        // Equip a new weapon, unequip the old one if necessary, and update the visuals and animator controller
        public void EquipWeapon(WeaponSO newItem)
        {
            WeaponSO oldItem = UnequipWeapon();

            _equippedWeaponSO = newItem;
            InstantiateWeaponPrefabs(newItem);
            UpdateAnimatorController(newItem);

            //_weaponChangedEventChannelSO.RaiseEvent(newItem, oldItem);
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
            if (_equippedWearablesSO != null)
            {
                for (int i = 0; i < _equippedWearablesSO.Length; i++)
                {
                    UnequipWearable(i);
                }
            }

            UnequipWeapon();

            EnableAllSkins();

            _playerAnimator.runtimeAnimatorController = _defaultAnimatorController;
        }

        // #region ISaveable Methods
        // public void LoadData(GameData data)
        // {
        //     if (_itemLookupEventSO.RaiseEvent(data.CurrentWeaponObjectIDs) is WeaponSO weapon)
        //     {
        //         _equippedWeaponSO = weapon;
        //     }
        //
        //     for (int i = 0; i < data.CurrentArmorObjectIDs.Length; i++)
        //     {
        //         if (_itemLookupEventSO.RaiseEvent(data.CurrentArmorObjectIDs[i]) is WearableSO armor)
        //         {
        //             _equippedWearablesSO[i] = armor;
        //         }
        //     }
        // }

        // public void SaveData(GameData data)
        // {
        //     if (_equippedWeaponSO != null)
        //     {
        //         data.CurrentWeaponObjectIDs = _equippedWeaponSO.ID;
        //     }
        //     else
        //     {
        //         data.CurrentWeaponObjectIDs = null;
        //     }
        //
        //     data.CurrentArmorObjectIDs = new string[_equippedWearablesSO.Length];
        //
        //     for (int i = 0; i < _equippedWearablesSO.Length; i++)
        //     {
        //         if (_equippedWearablesSO[i] != null)
        //         {
        //             data.CurrentArmorObjectIDs[i] = _equippedWearablesSO[i].ID;
        //         }
        //     }
        // }
        // #endregion

        // Instantiate and equip the weapon prefabs to the player's hands
        private void InstantiateWeaponPrefabs(WeaponSO newItem)
        {
            _equippedWeaponGameobjects.Clear();

            foreach (var gameObject in newItem.WeaponPrefabs)
            {
                if (WeaponToHandTransformMap.TryGetValue(gameObject, out var weaponHandTransform))
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

            int currentSceneIndex = 1;//_getCurrentSceneIndexEventChannelSO.RaiseEvent();

            if (currentSceneIndex == 2)
            {
                _playerAnimator.runtimeAnimatorController = newItem.CharacterMenuAnimatorOverrideController != null ? newItem.CharacterMenuAnimatorOverrideController : _defaultAnimatorController;
            }
            else if (currentSceneIndex == 1)
            {
                _playerAnimator.runtimeAnimatorController = newItem.GameplayAnimatorOverrideController != null ? newItem.GameplayAnimatorOverrideController : _defaultAnimatorController;
            }
        }

        // Handle requests to get the currently equipped wearable items
        private WearableSO[] GetEquippedWearables() => _equippedWearablesSO;

        // Handle requests to get the currently equipped weapon
        private WeaponSO GetEquippedWeapon() => _equippedWeaponSO;

        private void PlayerInitialization()
        {
            _playerAnimator = GetComponentInChildren<Animator>();

            _playerVisualTransform = _playerAnimator.transform;

            _playerHeadMesh = _playerVisualTransform.GetChild(1).GetComponent<SkinnedMeshRenderer>();
        }

        // Initialize arrays for storing equipped wearables and their corresponding meshes
        private void InitializeEquipmentSlots()
        {
            int numSlots = Enum.GetNames(typeof(WearableSlot)).Length;
            _equippedWearablesSO = new WearableSO[numSlots];
            _equippedWearableMeshes = new SkinnedMeshRenderer[numSlots];
        }

        // Set the default skin for a specific slot based on its index and activate/deactivate it
        private void ToggleIndividualSkin(int index, bool shouldSetActive)
        {
            if (Skins.TryGetValue((WearableSlot)index, out var skin))
            {
                skin.SetActive(shouldSetActive);
            }
        }

        // Activate all default skins
        private void EnableAllSkins()
        {
            foreach (var item in Skins)
            {
                item.Value.SetActive(true);
            }
        }
    }
}