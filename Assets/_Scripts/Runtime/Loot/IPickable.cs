using RPG.ScriptableObjects.Items;

namespace RPG.World
{
    public interface IPickable
    {
        ItemSO GetPickUpItem();
    }
}