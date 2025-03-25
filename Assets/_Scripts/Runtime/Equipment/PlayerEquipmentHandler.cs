using System;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace RPG
{
    public class PlayerEquipmentHandler : MonoBehaviour
    {
        [SerializeField] private PlayerEquipmentDataBase _playerEquipmentDataBase;

        [field: SerializeField]
        public SerializedDictionary<WearableSlot, GameObject> DefaultWearables { get; set; } = new();
        
        private WeaponSO _equippedWeaponSO;
        private WearableSO[] _equippedWearablesSO;
        
        private List<GameObject> _equippedWeaponGameobjects = new();
        private GameObject[] _equippedWearableObjects;
        
        
        [SerializeField] private Animator _playerAnimator;
        
        [SerializeField] private RuntimeAnimatorController _defaultAnimatorController;
        
        private void Awake()
        {
            if (DefaultWearables.ContainsKey(WearableSlot.Head) || DefaultWearables.ContainsKey(WearableSlot.Back))
            {
                DefaultWearables.Remove(WearableSlot.Head);
                DefaultWearables.Remove(WearableSlot.Back);
            }
            
            InitializeEquipmentSlots();
            EquipDefaultWearables();
        }

        private void EquipDefaultWearables()
        {
            foreach (var wearable in DefaultWearables)
            {
                if (_equippedWearablesSO[(int)WearableSlot.Torso] == null)
                {
                    if (DefaultWearables.TryGetValue(WearableSlot.Torso, out GameObject torsoObject))
                    {
                        torsoObject.SetActive(true);
                        _equippedWearableObjects[(int)WearableSlot.Torso] = torsoObject;
                    }
                }
                
                if (_equippedWearablesSO[(int)WearableSlot.Hands] == null)
                {
                    if (DefaultWearables.TryGetValue(WearableSlot.Hands, out GameObject handsObject))
                    {
                        handsObject.SetActive(true);
                        _equippedWearableObjects[(int)WearableSlot.Hands] = handsObject;
                    }
                }
                
                if (_equippedWearablesSO[(int)WearableSlot.Legs] == null)
                {
                    if (DefaultWearables.TryGetValue(WearableSlot.Legs, out GameObject legsObject))
                    {
                        legsObject.SetActive(true);
                        _equippedWearableObjects[(int)WearableSlot.Legs] = legsObject;
                    }
                }
                
                if (_equippedWearablesSO[(int)WearableSlot.Feet] == null)
                {
                    if (DefaultWearables.TryGetValue(WearableSlot.Feet, out GameObject feetObject))
                    {
                        feetObject.SetActive(true);
                        _equippedWearableObjects[(int)WearableSlot.Feet] = feetObject;
                    }
                }
            }
        }

        private void Start()
        {
            // if (_equippedWeaponSO != null)
            // {
            //     EquipWeapon(_equippedWeaponSO);
            // }
            //
            // if (_equippedWearablesSO != null)
            // {
            //     for (int i = 0; i < _equippedWearablesSO.Length; i++)
            //     {
            //         if (_equippedWearablesSO[i] != null)
            //         {
            //             EquipWearable(_equippedWearablesSO[i]);
            //         }
            //     }
            // }
        }

        private void InitializeEquipmentSlots()
        {
            int numSlots = Enum.GetNames(typeof(WearableSlot)).Length;
            _equippedWearablesSO = new WearableSO[numSlots];
            _equippedWearableObjects = new GameObject[numSlots];
        }
        
        public void EquipWearable(WearableSO newItem)
        {
            int slotIndex = (int)newItem.EquipSlot;
            WearableSO oldItem = UnequipWearable(slotIndex);
            
            ToggleDefaultWearable(slotIndex, false);
            
            _equippedWearablesSO[slotIndex] = newItem;
            
            var newObject = _playerEquipmentDataBase.GetEquipmentByID(newItem.GameObjectID);
            
            newObject.SetActive(true);
            
            _equippedWearableObjects[slotIndex] = newObject;

            //_wearableChangedEventChannelSO.RaiseEvent(newItem, oldItem);
        }
        
        public WearableSO UnequipWearable(int slotIndex)
        {
            var oldItem = _equippedWearablesSO[slotIndex];

            if (_equippedWearableObjects[slotIndex] != null)
            {
                _equippedWearableObjects[slotIndex].gameObject.SetActive(false);
            }

            _equippedWearablesSO[slotIndex] = null;

            if (_equippedWearablesSO[slotIndex] == null)
            {
                ToggleDefaultWearable(slotIndex, true);
            }
            return oldItem;
        }

        public void EquipWeapon(WeaponSO newItem)
        {
            WeaponSO oldItem = UnequipWeapon();

            _equippedWeaponSO = newItem;
            var newObject = _playerEquipmentDataBase.GetEquipmentByID(newItem.GameObjectID);
            
            UpdateAnimatorController(newItem);

            //_weaponChangedEventChannelSO.RaiseEvent(newItem, oldItem);
        }

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

            EnableAllDefaultWearbles();

            _playerAnimator.runtimeAnimatorController = _defaultAnimatorController;
        }
        
        private void UpdateAnimatorController(WeaponSO newItem)
        {
            // AnimatorOverrideController overrideController = _playerAnimator.runtimeAnimatorController as AnimatorOverrideController;
            //
            // int currentSceneIndex = _getCurrentSceneIndexEventChannelSO.RaiseEvent();
            //
            // if (currentSceneIndex == 2)
            // {
            //     _playerAnimator.runtimeAnimatorController = newItem.CharacterMenuAnimatorOverrideController != null ? newItem.CharacterMenuAnimatorOverrideController : _defaultAnimatorController;
            // }
            // else if (currentSceneIndex == 1)
            // {
            //     _playerAnimator.runtimeAnimatorController = newItem.GameplayAnimatorOverrideController != null ? newItem.GameplayAnimatorOverrideController : _defaultAnimatorController;
            // }
        }
        
        private void ToggleDefaultWearable(int index, bool shouldSetActive)
        {
            if (DefaultWearables.TryGetValue((WearableSlot)index, out var skin))
            {
                skin.SetActive(shouldSetActive);
            }
        }

        private void EnableAllDefaultWearbles()
        {
            foreach (var item in DefaultWearables)
            {
                item.Value.SetActive(true);
            }
        }
    }
}
