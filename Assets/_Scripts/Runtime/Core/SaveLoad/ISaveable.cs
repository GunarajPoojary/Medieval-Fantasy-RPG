namespace RPG.Core.SaveLoad
{
    public interface ISaveable
    {
        void LoadData(GameData data);

        void SaveData(GameData data);
    }
}