using UnityEngine;

namespace GunarajCode.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Food Item", menuName = "Inventory/Items/Food", order = 3)]
    public class FoodObject : ItemObject
    {
        public int Health;

        public override void Use()
        {
            throw new System.NotImplementedException();
        }
    }
}