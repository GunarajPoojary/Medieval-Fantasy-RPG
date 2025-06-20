using System;
using System.Collections.Generic;
using RPG.Events.EventChannel;
using RPG.Item;
using UnityEngine;

namespace RPG.EquipmentSystem
{
    [Serializable]
    public class EquipmentDatabase
    {
        public EquipmentSO EquipmentSO;
        public GameObject GameObject;
    }
    public class PlayerEquipmentManager : MonoBehaviour
    {
        [Header("Equipment Menu Components")]
        [Tooltip("The view component that displays the Equipment Menu UI")]
        [SerializeField] private RuntimeAnimatorController _defaultAnimator;
        [SerializeField] private ItemSOEventChannelSO _equipEquipmentEventChannel;
        [SerializeField] private EquipmentDatabase[] _equipmentDatabase;

        private PlayerEquipmentsController _controller;

        private void Awake()
        {
            Animator animator = GetComponent<Animator>();

            var equipmentEntries = new Dictionary<EquipmentSO, GameObject>(_equipmentDatabase.Length);

            foreach (EquipmentDatabase equipmentEntry in _equipmentDatabase)
            {
                if (equipmentEntry.EquipmentSO != null && equipmentEntry.GameObject != null)
                    equipmentEntries[equipmentEntry.EquipmentSO] = equipmentEntry.GameObject;
                else
                    Debug.LogWarning("Null key or value found in equipment dictionary.");
            }

            PlayerEquipmentDatabase playerEquipmentDatabase = new(equipmentEntries);

            _controller = new PlayerEquipmentsController(playerEquipmentDatabase, animator, _defaultAnimator);
        }

        private void OnEnable()
        {
            _equipEquipmentEventChannel.OnEventRaised += EquipEquipment;
        }

        private void OnDisable()
        {
            _equipEquipmentEventChannel.OnEventRaised -= EquipEquipment;
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
    }
}