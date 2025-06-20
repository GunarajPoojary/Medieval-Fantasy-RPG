using System;
using RPG.Item;
using UnityEngine;

namespace RPG.EquipmentSystem
{
    public class PlayerEquipmentsController
    {
        private readonly RuntimeAnimatorController _defaultAnimatorController;
        //[Space]
        private ArmorSO[] _equippedArmors;
        private WeaponSO _equippedWeapon;

        private Animator _animator;
        private readonly PlayerEquipmentDatabase _equipmentDataBase;

        public PlayerEquipmentsController(PlayerEquipmentDatabase playerEquipmentDatabase, Animator animator, RuntimeAnimatorController defaultAnimatorController)
        {
            _equipmentDataBase = playerEquipmentDatabase;
            _animator = animator;
            _defaultAnimatorController = defaultAnimatorController;

            InitializeEquipmentSlots();
        }

        private void Start()
        {
            // if (_equippedWeaponSO != null)
            // {
            //     EquipWeapon(_equippedWeaponSO);
            // }

            // if (_equippedWearablesSO != null)
            // {
            //     for (int i = 0; i < _equippedWearablesSO.Length; i++)
            //     {
            //         if (_equippedWearablesSO[i] != null)
            //         {
            //             EquipArmor(_equippedWearablesSO[i]);
            //         }
            //     }
            // }
        }

        // Equip a new wearable item, unequip the old one if necessary, and update the visuals
        public void EquipArmor(ArmorSO newArmor)
        {
            int slotIndex = (int)newArmor.EquipSlot;
            ArmorSO currentEquippedArmor = UnequipArmor(slotIndex);

            _equipmentDataBase.GetEquipmentObjectBySO(newArmor).SetActive(true);

            _equippedArmors[slotIndex] = newArmor;
        }

        // Unequip a wearable item from a specific slot, destroy the associated mesh, and reset the default skin
        public ArmorSO UnequipArmor(int slotIndex)
        {
            ArmorSO currentEquippedArmor = _equippedArmors[slotIndex];

            if (currentEquippedArmor != null)
                _equipmentDataBase.GetEquipmentObjectBySO(currentEquippedArmor).SetActive(false);

            _equippedArmors[slotIndex] = null;
            return currentEquippedArmor;
        }

        // Equip a new weapon, unequip the old one if necessary, and update the visuals and animator controller
        public void EquipWeapon(WeaponSO newWeapon)
        {
            WeaponSO currentEquippedWeapon = UnequipWeapon();

            _equipmentDataBase.GetEquipmentObjectBySO(newWeapon).SetActive(true);

            _equippedWeapon = newWeapon;
            UpdateAnimatorController();
        }

        // Unequip the currently equipped weapon, destroy its game objects, and reset the animator controller
        public WeaponSO UnequipWeapon()
        {
            if (_equippedWeapon == null) return null;

            WeaponSO currentEquippedWeapon = _equippedWeapon;

            _equipmentDataBase.GetEquipmentObjectBySO(currentEquippedWeapon).SetActive(false);

            _equippedWeapon = null;

            UpdateAnimatorController();

            return currentEquippedWeapon;
        }

        // Unequip all wearable items and weapons, and reset the player to the default skins and animator controller
        public void UnequipAll()
        {
            // if (_equippedWearablesSO != null)
            // {
            //     for (int i = 0; i < _equippedWearablesSO.Length; i++)
            //     {
            //         UnequipWearable(i);
            //     }
            // }

            UnequipWeapon();

            EnableAllSkins();

            _animator.runtimeAnimatorController = _defaultAnimatorController;
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

        private void InitializeEquipmentSlots()
        {
            int numSlots = Enum.GetNames(typeof(ArmorEquipSlot)).Length;
            _equippedArmors = new ArmorSO[numSlots];
        }

        // Update the animator controller based on the equipped weapon and current scene
        private void UpdateAnimatorController() => _animator.runtimeAnimatorController = _equippedWeapon != null ? _equippedWeapon.AnimatorOverrideController : _defaultAnimatorController;

        // Handle requests to get the currently equipped wearable items
        // private ArmorSO[] GetEquippedWearables() => _equippedWearablesSO;

        // // Handle requests to get the currently equipped weapon
        // private WeaponSO GetEquippedWeapon() => _equippedWeaponSO;

        // Initialize arrays for storing equipped wearables and their corresponding meshes

        // Activate all default skins
        private void EnableAllSkins()
        {
            // foreach (var item in Skins)
            // {
            //     item.Value.SetActive(true);
            // }
        }
    }
}