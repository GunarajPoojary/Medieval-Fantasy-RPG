using System;
using System.Collections.Generic;

namespace RPG.StatSystem
{
    public class CharacterStats
    {
        private readonly CharacterReadonlyBaseStatsSO _readonlyBaseStatsSO;
        protected readonly Dictionary<StatType, Stat> _stats;
        protected readonly Dictionary<StatType, float> _runtimeStats;
        protected readonly Action<StatType, Stat> _onStatChanged;
        protected readonly Action<StatType, float, float> _onRuntimeStatChanged;

        public CharacterStats(CharacterReadonlyBaseStatsSO readonlyBaseStatsSO, Action<StatType, Stat> onStatChanged, Action<StatType, float, float> onRuntimeStatChanged)
        {
            _readonlyBaseStatsSO = readonlyBaseStatsSO;
            _onStatChanged = onStatChanged;
            _onRuntimeStatChanged = onRuntimeStatChanged;

            _stats = new Dictionary<StatType, Stat>(_readonlyBaseStatsSO.ReadonlyBaseStats.Length);
            _runtimeStats = new Dictionary<StatType, float>(_readonlyBaseStatsSO.ReadonlyBaseStats.Length);

            foreach (ReadonlyBaseStat readonlyBaseStat in _readonlyBaseStatsSO.ReadonlyBaseStats)
            {
                Stat stat = new(readonlyBaseStat.value);
                _stats[readonlyBaseStat.statType] = stat;
                _runtimeStats[readonlyBaseStat.statType] = readonlyBaseStat.value;
                _onStatChanged?.Invoke(readonlyBaseStat.statType, stat);
                _onRuntimeStatChanged?.Invoke(readonlyBaseStat.statType, stat.Value, stat.Value);
            }
        }
    }
}