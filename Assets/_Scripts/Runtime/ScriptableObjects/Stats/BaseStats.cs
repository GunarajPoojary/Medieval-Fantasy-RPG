using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RPG
{
    /// <summary>
    /// Represents the base statistics for a character or item.
    /// </summary>
    [CreateAssetMenu(fileName = "New Stats", menuName = "RPG/Stats")]
    public class BaseStats : ScriptableObject
    {
        [SerializedDictionary("Stat Types", "Stat Values")]
        [SerializeField] private SerializedDictionary<StatType, int> _baseStatTypeToValueMap;

        public int GetStatValue(StatType statType) => _baseStatTypeToValueMap.TryGetValue(statType, out int value) ? value : 0;

        public SerializedDictionary<StatType, int> GetAllStats() => new SerializedDictionary<StatType, int>(_baseStatTypeToValueMap);

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

            foreach (var stat in _baseStatTypeToValueMap)
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