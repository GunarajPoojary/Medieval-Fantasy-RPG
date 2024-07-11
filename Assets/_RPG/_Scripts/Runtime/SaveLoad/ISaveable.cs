namespace RPG.SaveLoad
{
    public interface ISaveable
    {
        void LoadData(GameData data);
        void SaveData(GameData data);
    }
}