using UnityEngine;

namespace RPG.ScriptableObjects.Items
{
    [CreateAssetMenu(fileName = "New Food Item", menuName = "Inventory/Items/Food", order = 3)]
    public class FoodSO : ItemSO
    {
        public int HealthAmount;

        private void OnValidate() => Type = ItemType.Consumable;
    }
}