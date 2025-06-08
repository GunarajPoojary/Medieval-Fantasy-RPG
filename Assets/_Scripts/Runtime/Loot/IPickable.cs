namespace RPG.Loot
{
    public interface IPickable
    {
        void SetGameObject(bool isActive);
        ItemSO PickUpItem();
    }
}