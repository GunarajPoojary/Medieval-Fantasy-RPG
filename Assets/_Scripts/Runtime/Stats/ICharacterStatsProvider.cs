namespace RPG.Stats
{
    public interface ICharacterStatsProvider
    {
        float GetCurrentHealth();
        float GetCurrentDefense();
        float GetOverallHealth();
        float GetOverallDefense();
    }
}