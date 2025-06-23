using System;
using System.Collections.Generic;
using ProjectEmbersteel.Events.EventChannel;
using ProjectEmbersteel.Item;
using ProjectEmbersteel.StatSystem;
using UnityEngine;

namespace ProjectEmbersteel.EquipmentSystem
{
    [Serializable]
    public class EquipmentDatabase
    {
        public EquipmentSO equipmentSO;
        public Equipment equippable;
    }
    public class PlayerEquipmentManager : MonoBehaviour
    {
        public static PlayerEquipmentManager Instance { get; private set; }
        
        [Header("Equipment Menu Components")]
        [Tooltip("The view component that displays the Equipment Menu UI")]
        [SerializeField] private RuntimeAnimatorController _defaultAnimator;
        [SerializeField] private ItemSOEventChannelSO _equipEquipmentEventChannel;
        [SerializeField] private ItemSOEventChannelSO _unequipEquipmentEventChannel;

        [SerializeField] private EquipmentDatabase[] _equipmentDatabase;
        public PlayerEquipmentDatabase PlayerEquipmentDatabase { get; private set; }

        private PlayerEquipmentsController _controller;

        private void Awake() => Instance = this;

        private void Start() => InitializePlayerEquipmentsController();

        private void OnEnable() => SubscribeToEquipEquipmentEvent(true);

        private void OnDisable() => SubscribeToEquipEquipmentEvent(false);

        private void InitializePlayerEquipmentsController()
        {
            Animator animator = GetComponent<Animator>();
            IStatModifiable playerStats = GetComponentInParent<Player.Player>().StatModifiable;

            Dictionary<EquipmentSO, IEquippable> equipmentEntries = new(_equipmentDatabase.Length);

            foreach (EquipmentDatabase equipmentEntry in _equipmentDatabase)
            {
                if (equipmentEntry.equipmentSO != null && equipmentEntry.equippable != null)
                    equipmentEntries[equipmentEntry.equipmentSO] = equipmentEntry.equippable;
                else
                    Debug.LogWarning("Null key or value found in equipment dictionary.");
            }

            PlayerEquipmentDatabase = new(equipmentEntries);

            _controller = new PlayerEquipmentsController(playerStats, PlayerEquipmentDatabase, animator, _defaultAnimator);
        }

        private void SubscribeToEquipEquipmentEvent(bool subscribe)
        {
            if (subscribe)
            {
                _equipEquipmentEventChannel.OnEventRaised += EquipEquipment;
                _unequipEquipmentEventChannel.OnEventRaised += UnequipEquipment;
            }
            else
            {
                _equipEquipmentEventChannel.OnEventRaised -= EquipEquipment;
                _unequipEquipmentEventChannel.OnEventRaised -= UnequipEquipment;
            }
        }

        private void EquipEquipment(ItemSO equipment)
        {
            switch (equipment.Type)
            {
                case ItemType.OneHandedSword:
                case ItemType.GreatSword:
                case ItemType.BowAndArrow:
                    _controller.EquipWeapon(equipment as WeaponSO);
                    break;
                case ItemType.HeadArmor:
                case ItemType.ChestArmor:
                case ItemType.ArmArmor:
                case ItemType.BeltArmor:
                case ItemType.LegArmor:
                case ItemType.FeetArmor:
                    _controller.EquipArmor(equipment as ArmorSO);
                    break;
            }
        }

        private void UnequipEquipment(ItemSO equipment)
        {
            switch (equipment.Type)
            {
                case ItemType.OneHandedSword:
                case ItemType.GreatSword:
                case ItemType.BowAndArrow:
                    _controller.UnequipWeapon();
                    break;
                case ItemType.HeadArmor:
                case ItemType.ChestArmor:
                case ItemType.ArmArmor:
                case ItemType.BeltArmor:
                case ItemType.LegArmor:
                case ItemType.FeetArmor:
                    ArmorSO armor = equipment as ArmorSO;
                    _controller.UnequipArmor((int)armor.EquipSlot);
                    break;
            }
        }
    }
}