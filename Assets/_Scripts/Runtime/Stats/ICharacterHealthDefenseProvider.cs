namespace RPG.Stats
{
    public interface ICharacterHealthDefenseProvider
    {
        float GetCurrentHealth();
        float GetCurrentDefense();
        float GetOverallHealth();
        float GetOverallDefense();
    }
}