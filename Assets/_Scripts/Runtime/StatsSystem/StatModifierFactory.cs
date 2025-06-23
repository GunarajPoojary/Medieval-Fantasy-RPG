using System;

namespace ProjectEmbersteel.StatSystem
{
    public interface IStatModifierFactory
    {
        StatModifier Create(StatValueType statValueType, ModifierOperationType operationType, StatType statType, float value);
    }

    public class StatModifierFactory : IStatModifierFactory
    {
        public StatModifier Create(StatValueType statValueType, ModifierOperationType operationType, StatType statType, float value)
        {
            IModifierOperationStrategy strategy = statValueType switch
            {
                StatValueType.Flat => new FlatOperation(operationType, value),
                StatValueType.Percentage => new PercentOperation(operationType, value),
                _ => throw new ArgumentOutOfRangeException()
            };

            return new StatModifier(statType, strategy);
        }
    }
}