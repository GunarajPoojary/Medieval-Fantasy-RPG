using System.Collections.Generic;
using UnityEngine;

namespace RPG.StatSystem
{
    [CreateAssetMenu(fileName = "NewEquipmentStats", menuName = "Custom/Stats/EquipmentStats")]
    public class EquipmentStatsSO : DescriptionBaseSO
    {
        [field: SerializeField] public EquipmentBaseStatsSO BaseStats { get; private set; }
        private readonly List<StatModifier> _statModifiers = new();

        private void OnEnable() => Initialize();

        private void Initialize()
        {
            StatModifierFactory statModifierFactory = new();

            _statModifiers.Add(statModifierFactory.Create(BaseStats.PrimaryBaseStat.statValueType,
                                                          ModifierOperationType.Bonus,
                                                          BaseStats.PrimaryBaseStat.statType,
                                                          BaseStats.PrimaryBaseStat.value));

            foreach (ReadonlyBaseStat stat in BaseStats.SecondaryBaseStats)
                _statModifiers.Add(statModifierFactory.Create(stat.statValueType, ModifierOperationType.Bonus, stat.statType, stat.value));
        }

        public void AddBonuses(IStatModifiable statModifiable)
        {
            foreach (StatModifier modifier in _statModifiers)
                statModifiable.AddStatModifier(modifier);
        }

        public void RemoveBonuses(IStatModifiable statModifiable)
        {
            foreach (StatModifier modifier in _statModifiers)
                statModifiable.RemoveStatModifier(modifier);
        }
    }
}