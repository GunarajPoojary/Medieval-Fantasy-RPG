using UnityEngine;

namespace RPG.StatSystem
{
    [CreateAssetMenu(fileName = "NewEquipmentBaseStats", menuName = "Custom/Stats/BaseStats/EquipmentBaseStats")]
    public class EquipmentBaseStatsSO : DescriptionBaseSO
    {
        [field: SerializeField] public ReadonlyBaseStat PrimaryBaseStat { get; private set; }
        [field: SerializeField] public ReadonlyBaseStat[] SecondaryBaseStats { get; private set; }
    }
}