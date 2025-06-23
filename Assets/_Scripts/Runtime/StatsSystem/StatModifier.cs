using System;

namespace RPG.StatSystem
{
    public enum ModifierOperationType
    {
        Bonus = 1,
        Penalty = -1
    }

    /// <summary>
    /// Represents a single modifier that can be applied to a stat.
    /// Implements IComparable for consistent ordering during calculation.
    /// </summary>
    public class StatModifier
    {
        public readonly StatType type;
        private readonly IModifierOperationStrategy _operationType;

        /// <summary>
        /// Creates a new stat modifier.
        /// </summary>
        /// <param name="value">The modifier value</param>
        /// <param name="type">How the modifier affects the calculation</param>
        /// <param name="order">Calculation order (lower values calculated first)</param>
        /// <param name="source">Optional source object for tracking purposes</param>
        public StatModifier(StatType type, IModifierOperationStrategy operationType)
        {
            this.type = type;
            _operationType = operationType;
        }

        public float GetModifiedStatValue(float value) => _operationType.Calculate(value);
    }
}
