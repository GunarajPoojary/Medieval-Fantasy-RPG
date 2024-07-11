using AYellowpaper.SerializedCollections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace RPG.Gameplay.Stats
{
    [System.Serializable]
    public enum StatType
    {
        Health,
        Defense,
        Attack,
        AttackSpeed,
        Durability,
        MovementSpeed,
        MinRange,
        MaxRange
    }

    [CreateAssetMenu(fileName = "New BaseStats", menuName = "RPG/BaseStats")]
    public class BaseStats : ScriptableObject
    {
        [SerializedDictionary("Stat Types", "Stat Values")]
        public SerializedDictionary<StatType, int> BaseStatTypeToValueMap = new();

        public int GetStatValue(StatType statType) => BaseStatTypeToValueMap.TryGetValue(statType, out int value) ? value : 0;

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
            foreach (var stat in BaseStatTypeToValueMap)
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
