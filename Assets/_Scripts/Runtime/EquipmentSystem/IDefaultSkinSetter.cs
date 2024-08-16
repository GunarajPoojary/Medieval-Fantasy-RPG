namespace RPG.EquipmentSystem
{
    // Interface for setting the default skin
    public interface IDefaultSkinSetter
    {
        void SetDefaultSkin(int index, bool shouldSetActive);
    }
}