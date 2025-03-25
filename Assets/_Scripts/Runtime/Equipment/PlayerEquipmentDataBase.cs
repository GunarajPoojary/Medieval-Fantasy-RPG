using System.Collections.Generic;
using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace RPG
{
    public class PlayerEquipmentDataBase : MonoBehaviour
    {
        [SerializeField] private SerializedDictionary<string, GameObject> _equipmentsLookup = new();

        public GameObject GetEquipmentByID(string gameObjectID) => _equipmentsLookup.GetValueOrDefault(gameObjectID);
    }
}