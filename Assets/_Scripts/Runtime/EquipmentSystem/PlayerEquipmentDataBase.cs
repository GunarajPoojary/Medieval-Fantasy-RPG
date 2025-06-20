using System.Collections.Generic;
using RPG.Item;
using UnityEngine;

namespace RPG.EquipmentSystem
{
    public class PlayerEquipmentDatabase
    {
        private readonly Dictionary<EquipmentSO, GameObject> _equipmentLookup;

        public PlayerEquipmentDatabase(Dictionary<EquipmentSO, GameObject> equipmentEntries)
        {
            // Initialize the lookup table for fast access
            _equipmentLookup = equipmentEntries;
        }

        /// <summary>
        /// Gets the corresponding GameObject (already in the scene) for the given EquipmentSO.
        /// </summary>
        public GameObject GetEquipmentObjectBySO(EquipmentSO equipmentSO)
        {
            if (_equipmentLookup == null)
            {
                Debug.LogError("Equipment lookup table not initialized.");
                return null;
            }

            if (equipmentSO == null)
            {
                Debug.LogError("Provided EquipmentSO is null.");
                return null;
            }

            if (_equipmentLookup.TryGetValue(equipmentSO, out GameObject obj))
                return obj;

            Debug.LogWarning($"EquipmentSO not found in lookup: {equipmentSO.name}");
            return null;
        }
    }
}