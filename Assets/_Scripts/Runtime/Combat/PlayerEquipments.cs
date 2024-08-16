using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace RPG.Combat
{
    /// <summary>
    /// Manages the player's equipped items by mapping weapon prefabs to hand transforms.
    /// </summary>
    public class PlayerEquipments : MonoBehaviour
    {
        [SerializedDictionary("Weapon Prefab", "Hand Transform")]
        [field: SerializeField] public SerializedDictionary<GameObject, Transform> Equipments { get; private set; }
    }
}