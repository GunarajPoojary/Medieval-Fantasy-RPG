using AYellowpaper.SerializedCollections;

namespace RPG.Gameplay.Stats
{
    [System.Serializable]
    public class StatModifier
    {
        public SerializedDictionary<StatType, int> RuntimeStatTypeToValueMap { get; set; }

        public StatModifier(BaseStats baseStats)
        {
            RuntimeStatTypeToValueMap = new();

            foreach (var stat in baseStats.BaseStatTypeToValueMap)
                RuntimeStatTypeToValueMap[stat.Key] = stat.Value;
        }

        public int GetStatValue(StatType statType) => RuntimeStatTypeToValueMap.TryGetValue(statType, out int value) ? value : 0;

        public void SetStatValue(StatType statType, int amount)
        {
            if (RuntimeStatTypeToValueMap.ContainsKey(statType))
                RuntimeStatTypeToValueMap[statType] = amount;
        }

        public void ChangeStatValue(StatType statType, int amount)
        {
            if (RuntimeStatTypeToValueMap.ContainsKey(statType))
                RuntimeStatTypeToValueMap[statType] += amount;
            else
                UnityEngine.Debug.Log("Stat Not Found");
        }
    }
}
