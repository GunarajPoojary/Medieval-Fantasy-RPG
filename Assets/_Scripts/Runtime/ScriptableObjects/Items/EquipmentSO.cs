using UnityEngine;

namespace RPG.Item
{
    public abstract class EquipmentSO : ItemSO
    {
        //public BaseStats EquipmentStats;

        [Tooltip("Can be found in Player's EquipmentManager Component")]
        public string GameObjectID;
    }
}