using ProjectEmbersteel.Item;

namespace ProjectEmbersteel.Loot
{
    /// <summary>
    /// Contract for pickable item
    /// </summary>
    public interface IPickable
    {
        void SetGameObject(bool isActive);
        ItemSO PickUpItem();
    }
}