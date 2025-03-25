using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RPG
{
    /// <summary>
    /// Holds the runtime statistics of a character, allowing for dynamic stat changes during gameplay.
    /// </summary>
    [CreateAssetMenu(fileName = "New Runtime Stats", menuName = "RPG/Runtime Stats")]
    public class RuntimeStats : ScriptableObject
    {
        [SerializedDictionary("Stat Types", "Stat Values")]
        public SerializedDictionary<StatType, int> OverallStatTypeToValueMap { get; set; } = new SerializedDictionary<StatType, int>();

        [SerializedDictionary("Stat Types", "Stat Values")]
        public SerializedDictionary<StatType, int> CurrentStatTypeToValueMap { get; set; } = new SerializedDictionary<StatType, int>();

        public int GetOverallStatValue(StatType statType) => OverallStatTypeToValueMap.TryGetValue(statType, out int value) ? value : 0;

        public int GetCurrentStatValue(StatType statType) => CurrentStatTypeToValueMap.TryGetValue(statType, out int value) ? value : 0;

        public void SetCurrentStatValue(StatType statType, int amount)
        {
            if (CurrentStatTypeToValueMap.ContainsKey(statType))
            {
                CurrentStatTypeToValueMap[statType] = amount;
            }
        }

        public void ChangeCurrentStatValue(StatType statType, int amount)
        {
            if (CurrentStatTypeToValueMap.ContainsKey(statType))
            {
                CurrentStatTypeToValueMap[statType] += amount;
            }
        }

        public void ChangeOverallStatValue(StatType statType, int amount)
        {
            if (OverallStatTypeToValueMap.ContainsKey(statType))
            {
                OverallStatTypeToValueMap[statType] += amount;
            }
        }

        public void ResetCurrentStats(SerializedDictionary<StatType, int> baseStats) => CurrentStatTypeToValueMap = new SerializedDictionary<StatType, int>(baseStats);

        public void ResetOverallStats(SerializedDictionary<StatType, int> baseStatTypeToValueMap) => OverallStatTypeToValueMap = new SerializedDictionary<StatType, int>(baseStatTypeToValueMap);

        public override string ToString()
        {
            var displayNames = new Dictionary<StatType, string>
            {
                { StatType.Health, "Health" },
                { StatType.Defense, "Defense" },
                { StatType.Attack, "Attack" },
                { StatType.AttackSpeed, "Attack Speed" },
                { StatType.Durability, "Durability" },
                { StatType.MovementSpeed, "Movement Speed" },
                { StatType.MinRange, "Min Range" },
                { StatType.MaxRange, "Max Range" }
            };

            const int maxStatTypeLength = 15;

            var stringBuilder = new StringBuilder();

            foreach (var stat in OverallStatTypeToValueMap)
            {
                if (displayNames.TryGetValue(stat.Key, out string displayName))
                {
                    string value = stat.Value.ToString("D2");
                    stringBuilder.AppendLine($"{displayName.PadRight(maxStatTypeLength)} = {value}");
                }
            }

            return stringBuilder.ToString();
        }
    }
}