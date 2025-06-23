using System;
using UnityEngine;

namespace ProjectEmbersteel.StatSystem
{
    public interface IModifierOperationStrategy
    {
        float Calculate(float value);
    }

    public class FlatOperation : IModifierOperationStrategy
    {
        private readonly float _flatValue;
        private readonly ModifierOperationType _operationType;

        public FlatOperation(ModifierOperationType operationType, float flatValue)
        {
            _flatValue = flatValue;
            _operationType = operationType;
        }

        public float Calculate(float value) => (float)_operationType * _flatValue;
    }

    public class PercentOperation : IModifierOperationStrategy
    {
        private readonly float _percentValue;
        private readonly ModifierOperationType _operationType;

        public PercentOperation(ModifierOperationType operationType, float percentValue)
        {
            _percentValue = percentValue;
            _operationType = operationType;
        }

        public float Calculate(float value) => (float)_operationType * ((float)Math.Round(value * _percentValue / 100, 2));
    }
}
