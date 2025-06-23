using System;
using UnityEngine;

namespace ProjectEmbersteel.StatSystem
{
    public class PlayerStats : CharacterStats, IStatModifiable
    {
        public PlayerStats(CharacterReadonlyBaseStatsSO baseStatsSO,
                           Action<StatType,
                           Stat> onStatChanged, Action<StatType,
                           float, float> onRuntimeStatChanged) : base(baseStatsSO,
                           onStatChanged, onRuntimeStatChanged)
        { }

        public void AddStatModifier(StatModifier modifier)
        {
            if (!_stats.TryGetValue(modifier.type, out Stat runtimeStat)) return;

            runtimeStat.AddStatModifier(modifier);
            _onStatChanged?.Invoke(modifier.type, runtimeStat);
        }

        public void RemoveStatModifier(StatModifier modifier)
        {
            if (!_stats.TryGetValue(modifier.type, out Stat stat)) return;

            stat.RemoveStatModifier(modifier);
            _onStatChanged?.Invoke(modifier.type, stat);
        }

        public void HandleDamage(float attack)
        {
            if (!_runtimeStats.TryGetValue(
                StatType.DEF,
                out float currentDEF)) return;

            if (!_runtimeStats.TryGetValue(
            StatType.HP,
            out float currentHP)) return;

            float damage = CalculateDamage(attack, currentDEF);

            // Apply damage to HP
            currentHP -= damage;
            currentHP = Mathf.Max(currentHP, 0);

            float armorDegradationRatio = 0.1f;

            // Degrade armor based on a percentage of damage taken
            float armorDamage = damage * armorDegradationRatio;
            currentDEF -= armorDamage;
            currentDEF = Mathf.Max(currentDEF, 0); // Don't allow negative defense

            _runtimeStats[StatType.HP] = currentHP;
            _runtimeStats[StatType.DEF] = currentDEF;

            if (!_stats.TryGetValue(
                StatType.DEF,
                out Stat dEF)) return;

            if (!_stats.TryGetValue(
            StatType.HP,
            out Stat hP)) return;

            if (currentHP == 0) Die();

            _onRuntimeStatChanged?.Invoke(StatType.HP, currentHP, hP.Value);
            _onRuntimeStatChanged?.Invoke(StatType.DEF, currentDEF, dEF.Value);
        }

        private void Die()
        {
            //
        }

        public void RestoreRuntimeStat(StatType statType, float currentValue)
        {
            //
        }

        /// <summary>
        /// Scaled damage formula: Damage = ATK * (ATK / (ATK + DEF))
        /// </summary>
        private float CalculateDamage(float atk, float def)
        {
            atk = Mathf.Max(atk, 0);
            def = Mathf.Max(def, 0);
            float damage = atk * (atk / (atk + def));
            return Mathf.Max(1f, damage); // Always deal at least 1 damage
        }
    }
}