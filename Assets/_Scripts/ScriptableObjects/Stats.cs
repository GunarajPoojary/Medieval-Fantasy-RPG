using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace GunarajCode
{
    /// <summary>
    /// Represents a struct containing information about a particular stat, including its type and value.
    /// </summary>
    [System.Serializable]
    public struct StatInfo
    {
        // The type of the stat
        public StatType StatType;
        // The value of the stat
        public int StatValue;
    }

    public enum StatType { Health, Defense, Attack, AttackSpeed, Durability, MovementSpeed, MinRange, MaxRange }

    /// <summary>
    /// Represents a collection of stats that can be associated with game objects.
    /// </summary>
    [CreateAssetMenu(fileName = "New Stats")]
    public class Stats : ScriptableObject
    {
        [SerializeField] private List<StatInfo> _statInfo = new List<StatInfo>();
        // Dictionary to store stat type-value pairs
        public Dictionary<StatType, int> StatTypeToValueMap { get; private set; } = new Dictionary<StatType, int>();

        private void OnEnable() => InitializeStatsDictionary();

        private void InitializeStatsDictionary()
        {
            StatTypeToValueMap = new Dictionary<StatType, int>();
            _statInfo.ForEach(X => StatTypeToValueMap.Add(X.StatType, X.StatValue));
        }

        /// <summary>
        /// Gets the value of a specific stat.
        /// </summary>
        /// <param name="stat">The type of the stat to retrieve.</param>
        /// <returns>The value of the specified stat.</returns>
        public int GetStatValue(StatType stat)
        {
            if (StatTypeToValueMap.TryGetValue(stat, out int statValue))
                return statValue;

            Debug.LogError("No Stat");
            return 0;
        }

        /// <summary>
        /// Changes the value of a specific stat by a specified amount.
        /// </summary>
        /// <param name="stat">The type of the stat to change.</param>
        /// <param name="amount">The amount by which to change the stat value.</param>
        public void ChangeStatValue(StatType stat, int amount)
        {
            if (StatTypeToValueMap.TryGetValue(stat, out int statValue))
                StatTypeToValueMap[stat] += amount;
        }

        public override string ToString()
        {
            Dictionary<StatType, string> displayNames = new Dictionary<StatType, string>
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

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var stat in StatTypeToValueMap)
            {
                if (displayNames.TryGetValue(stat.Key, out string displayName))
                {
                    string value = stat.Value.ToString();

                    stringBuilder.AppendLine($"{displayName.PadRight(maxStatTypeLength)} = {(value.Length == 1 ? value.PadLeft(2, '0') : value.PadLeft(2))}");
                }
            }
            return stringBuilder.ToString();
        }
    }
}
