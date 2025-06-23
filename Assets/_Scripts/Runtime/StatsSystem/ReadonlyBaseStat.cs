namespace RPG.StatSystem
{
    /// <summary>
    /// Defines the available stat types for the system.
    /// Can be extended to include additional stat types as needed.
    /// </summary>
    public enum StatType
    {
        HP,
        ATK,
        DEF
    }

    public enum StatValueType
    {
        Flat,
        Percentage
    }

    [System.Serializable]
    public struct ReadonlyBaseStat
    {
        public StatType statType;
        public StatValueType statValueType;
        public float value;
    }
}