using System.Collections.Generic;

namespace RPG.StatSystem
{
    public class Stat
    {
        private readonly float _value;
        private float _lastCalculatedValue;
        private bool _isDirty = true;
        private readonly List<StatModifier> _modifiers = new();

        public float Value
        {
            get
            {
                if (_isDirty)
                    RecalculateValue();

                return _lastCalculatedValue;
            }
        }

        public Stat(float value)
        {
            _value = value;
        }

        public void AddStatModifier(StatModifier modifier)
        {
            if (modifier == null) return;

            _modifiers.Add(modifier);
            _isDirty = true;
        }

        public void RemoveStatModifier(StatModifier modifier)
        {
            if (modifier == null) return;
            
            _modifiers.Remove(modifier);
            _isDirty = true;
        }

        private void RecalculateValue()
        {
            if (_modifiers == null) return;

            float summedValue = 0;

            foreach (StatModifier modifier in _modifiers)
                summedValue += modifier.GetModifiedStatValue(_value);

            _lastCalculatedValue = _value + summedValue;

            _isDirty = false;
        }
    }
}