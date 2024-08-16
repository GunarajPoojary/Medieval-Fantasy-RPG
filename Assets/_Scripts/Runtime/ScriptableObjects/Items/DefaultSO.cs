using UnityEngine;

namespace RPG.ScriptableObjects.Items
{
    [CreateAssetMenu(fileName = "New Default Item", menuName = "Inventory/Items/Default")]
    public class DefaultSO : ItemSO
    {
        private void OnValidate() => Type = ItemType.Default;
    }
}