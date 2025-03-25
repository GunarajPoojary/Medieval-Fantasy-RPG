namespace RPG
{
    public interface ICharacterStatsProvider
    {
        float GetCurrentHealth();
        float GetCurrentDefense();
        float GetOverallHealth();
        float GetOverallDefense();
    }
}